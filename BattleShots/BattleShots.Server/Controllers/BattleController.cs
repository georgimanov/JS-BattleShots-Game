using BattleShots.Models;
using BattleShots.Server.Models;
using BattleShots.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BattleShots.Server.Controllers
{
    public class BattleController : BaseApiController
    {
        private const string OpenState = "open";
        private const string InProgressState = "in-progress";
        private const string GameReadyState = "game-ready";
        private const string BattleStartedState = "battle-started";
        private const string GameFinishedState = "finished";

        private const string PartialState = "partial";
        private const string DestroyedState = "destroyed";

        private Random random = new Random();

        // POST api/battle/place/5
        [HttpPost]
        public IHttpActionResult Place(int id, [FromBody]FiguresPlacingModel placingModel)
        {
            var response = this.PerformOperation(() =>
            {
                string sessionKey = GetSessionKey();
                if (!string.IsNullOrEmpty(placingModel.Password))
                {
                    ValidatePassword(placingModel.Password);
                }

                var context = new ApplicationDbContext();
                using (context)
                {
                    var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                    if (user == null)
                    {
                        throw new ServerErrorException("The user does not exist.", ErrorType.InvalidUser);
                    }

                    var game = context.Games.FirstOrDefault(g => g.Id == id);
                    var stateInProgress = context.GameStates.First(s => s.State == InProgressState);
                    if (game == null)
                    {
                        throw new ServerErrorException("The game does not exist.", ErrorType.InvalidGame);
                    }

                    if (!string.IsNullOrEmpty(game.Password) && game.Password != placingModel.Password)
                    {
                        throw new ServerErrorException("The entered password for this game is wrong.", ErrorType.InvalidPassword);
                    }

                    if (game.State != stateInProgress)
                    {
                        throw new ServerErrorException("The game is not currently in progress.", ErrorType.InvalidGame);
                    }

                    int playerNumber = 0;
                    if (game.FirstPlayer != user)
                    {
                        if (game.SecondPlayer != user)
                        {
                            throw new ServerErrorException("You cannot play in this game.", ErrorType.InvalidGame);
                        }
                        else
                        {
                            playerNumber = 2;
                        }
                    }
                    else
                    {
                        playerNumber = 1;
                    }

                    char[,] board = BoardUtilities.GenerateEmptyBoard();
                    var units = new List<Unit>();
                    foreach (var model in placingModel.Units)
                    {
                        board = BoardUtilities.PlaceUnit(board, model);
                        var type = context.UnitTypes.First(t => t.Type == model.UnitType);
                        var state = context.UnitStates.First(t => t.State == Ship.FullState);
                        var rotation = context.Rotations.First(r => r.Type == model.Rotation);
                        Unit unit = new Unit()
                        {
                            User = playerNumber == 1 ? game.FirstPlayer : game.SecondPlayer,
                            Type = type,
                            State = state,
                            Row = model.Row,
                            Col = model.Col,
                            Rotation = rotation,
                            Game = game
                        };

                        switch (model.UnitType)
                        {
                            case Ship.AircraftCarrier:
                                unit.RemainingCells = Ship.AircraftCarrierLength;
                                break;
                            case Ship.Battleship:
                                unit.RemainingCells = Ship.BattleshipLength;
                                break;
                            case Ship.Destroyer:
                                unit.RemainingCells = Ship.DestroyerLength;
                                break;
                            case Ship.Submarine:
                                unit.RemainingCells = Ship.SubmarineLength;
                                break;
                            case Ship.PatrolBoat:
                                unit.RemainingCells = Ship.PatrolBoatLength;
                                break;
                        }

                        units.Add(unit);
                    }

                    if (playerNumber == 1)
                    {
                        game.FirstPlayerBoard = BoardUtilities.SerializeBoard(board);
                        game.FirstPlayerUnits = units;
                    }
                    else if (playerNumber == 2)
                    {
                        game.SecondPlayerBoard = BoardUtilities.SerializeBoard(board);
                        game.SecondPlayerUnits = units;
                    }

                    game.FirstPlayerVisibleBoard = BoardUtilities.SerializeBoard(BoardUtilities.GenerateEmptyBoard());
                    game.SecondPlayerVisibleBoard = BoardUtilities.SerializeBoard(BoardUtilities.GenerateEmptyBoard());

                    context.SaveChanges();
                    game = context.Games.FirstOrDefault(g => g.Id == id);
                    if (game.FirstPlayerBoard != null && game.SecondPlayerBoard != null)
                    {
                        var state = context.GameStates.First(s => s.State == GameReadyState);
                        game.State = state;
                    }

                    context.SaveChanges();

                    return GetGameModel(game, playerNumber);
                }
            });

            return response;
        }

        // GET api/battle/state/5
        [HttpGet]
        public IHttpActionResult State(int id)
        {
            var response = this.PerformOperation(() =>
            {
                string sessionKey = GetSessionKey();

                var context = new ApplicationDbContext();
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ServerErrorException("The user does not exist.", ErrorType.InvalidUser);
                }

                var game = context.Games.FirstOrDefault(g => g.Id == id);
                var stateGameReady = context.GameStates.First(s => s.State == GameReadyState);
                var stateBattleStarted = context.GameStates.First(s => s.State == BattleStartedState);
                if (game == null)
                {
                    throw new ServerErrorException("The game does not exist.", ErrorType.InvalidGame);
                }

                int playerNumber = 0;
                if (game.FirstPlayer != user)
                {
                    if (game.SecondPlayer != user)
                    {
                        throw new ServerErrorException("You cannot play in this game.", ErrorType.InvalidGame);
                    }
                    else
                    {
                        playerNumber = 2;
                    }
                }
                else
                {
                    playerNumber = 1;
                }

                return GetGameModel(game, playerNumber);
            });

            return response;
        }

        // POST api/battle/attack/5
        [HttpPost]
        public IHttpActionResult Attack(int id, [FromBody]AttackingModel attackingModel)
        {
            var response = this.PerformOperation(() =>
            {
                string sessionKey = GetSessionKey();

                BoardUtilities.ValidateAttack(attackingModel);

                var context = new ApplicationDbContext();
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ServerErrorException("The user does not exist.", ErrorType.InvalidUser);
                }

                var game = context.Games.FirstOrDefault(g => g.Id == id);
                var stateBattleStarted = context.GameStates.First(s => s.State == BattleStartedState);
                var stateGameReady = context.GameStates.First(s => s.State == GameReadyState);
                if (game == null)
                {
                    throw new ServerErrorException("The game does not exist.", ErrorType.InvalidGame);
                }

                if (game.State != stateBattleStarted && game.State != stateGameReady)
                {
                    throw new ServerErrorException("The game has not currently started.", ErrorType.InvalidGame);
                }

                int playerNumber = 0;
                if (game.FirstPlayer != user)
                {
                    if (game.SecondPlayer != user)
                    {
                        throw new ServerErrorException("You cannot play in this game.", ErrorType.InvalidGame);
                    }
                    else
                    {
                        playerNumber = 2;
                    }
                }
                else
                {
                    playerNumber = 1;
                }

                if ((game.CurrentTurn % 2 == 0 && playerNumber == 1) || (game.CurrentTurn % 2 != 0 && playerNumber == 2))
                {
                    throw new ServerErrorException("It is not currently your turn.", ErrorType.InvalidInput);
                }

                char[,] attackedBoard;
                if (playerNumber == 1)
                {
                    attackedBoard = BoardUtilities.DeserializeBoard(game.SecondPlayerBoard);
                }
                else
                {
                    attackedBoard = BoardUtilities.DeserializeBoard(game.FirstPlayerBoard);
                }

                char attackedCell = BoardUtilities.Attack(attackedBoard, attackingModel);
                if (attackedCell != '\0')
                {
                    attackedBoard[attackingModel.Row - 1, attackingModel.Col - 1] = attackedCell;
                    if (attackedCell != Cell.HitWaterBody)
                    {
                        Unit unit = GetUnit(game, playerNumber, attackedCell);
                        if (unit == null)
                        {
                            throw new ServerErrorException("Invalid unit,", ErrorType.InvalidInput);
                        }

                        var partialState = context.UnitStates.FirstOrDefault(s => s.State == PartialState);
                        var destroyedState = context.UnitStates.FirstOrDefault(s => s.State == DestroyedState);
                        unit.RemainingCells--;
                        if (unit.RemainingCells <= 0)
                        {
                            unit.State = destroyedState;
                        }
                        else
                        {
                            unit.State = partialState;
                        }

                        game.CurrentTurn--;
                    }
                }
                else
                {
                    game.CurrentTurn--;
                }

                game.CurrentTurn++;
                if (playerNumber == 1)
                {
                    game.SecondPlayerBoard = BoardUtilities.SerializeBoard(attackedBoard);
                    game.SecondPlayerVisibleBoard = BoardUtilities.SerializeBoard(BoardUtilities.GenerateVisibleBoard(attackedBoard));
                }
                else
                {
                    game.FirstPlayerBoard = BoardUtilities.SerializeBoard(attackedBoard);
                    game.FirstPlayerVisibleBoard = BoardUtilities.SerializeBoard(BoardUtilities.GenerateVisibleBoard(attackedBoard));
                }

                int winner = CheckWinner(game, playerNumber);
                if (winner != 0)
                {
                    var state = context.GameStates.FirstOrDefault(s => s.State == GameFinishedState);
                    game.State = state;
                    if (winner == 1)
                    {
                        game.FirstPlayer.Score++;
                    }
                    else if (winner == 2)
                    {
                        game.SecondPlayer.Score++;
                    }
                }

                context.SaveChanges();

                return GetGameModel(game, playerNumber);
            });

            return response;
        }

        // POST api/battle/random/5
        [HttpPost]
        public IHttpActionResult Random(int id)
        {
            var response = this.PerformOperationWithNoContent(() =>
            {
                var context = new ApplicationDbContext();
                using (context)
                {
                    var game = context.Games.FirstOrDefault(g => g.Id == id);
                    var stateGameReady = context.GameStates.First(s => s.State == GameReadyState);
                    var stateOpen = context.GameStates.First(s => s.State == OpenState);
                    var stateInProgress = context.GameStates.First(s => s.State == InProgressState);
                    if (game == null)
                    {
                        throw new ServerErrorException("The game does not exist.", ErrorType.InvalidGame);
                    }
                    if (game.State != stateGameReady && game.State != stateOpen && game.State != stateInProgress)
                    {
                        throw new ServerErrorException("The game is not ready to start.", ErrorType.InvalidGame);
                    }
                    if (game.FirstPlayerBoard != null && game.SecondPlayerBoard != null)
                    {
                        return "";
                    }

                    var first = BoardUtilities.GenerateRandomBoard();
                    var second = BoardUtilities.GenerateRandomBoard();
                    game.FirstPlayerBoard = BoardUtilities.SerializeBoard(first.Item1);
                    var fullState = context.UnitStates.FirstOrDefault(s => s.State == "full");
                    foreach (var unit in first.Item2)
                    {
                        var type = context.UnitTypes.FirstOrDefault(s => s.Type == unit.UnitType);
                        var rotation = context.Rotations.FirstOrDefault(r => r.Type == unit.Rotation);
                        var unitToAdd = new Unit()
                        {
                            User = game.FirstPlayer,
                            Type = type,
                            State = fullState,
                            Row = unit.Row,
                            Col = unit.Col,
                            Rotation = rotation,
                            RemainingCells = BoardUtilities.GetLength(unit.UnitType),
                            Game = game
                        };

                        game.FirstPlayerUnits.Add(unitToAdd);
                    }

                    game.SecondPlayerBoard = BoardUtilities.SerializeBoard(second.Item1);
                    foreach (var unit in second.Item2)
                    {
                        var type = context.UnitTypes.FirstOrDefault(s => s.Type == unit.UnitType);
                        var rotation = context.Rotations.FirstOrDefault(r => r.Type == unit.Rotation);
                        var unitToAdd = new Unit()
                        {
                            User = game.SecondPlayer,
                            Type = type,
                            State = fullState,
                            Row = unit.Row,
                            Col = unit.Col,
                            Rotation = rotation,
                            RemainingCells = BoardUtilities.GetLength(unit.UnitType),
                            Game = game
                        };

                        game.SecondPlayerUnits.Add(unitToAdd);
                    }

                    game.FirstPlayerVisibleBoard = BoardUtilities.SerializeBoard(BoardUtilities.GenerateVisibleBoard(first.Item1));
                    game.SecondPlayerVisibleBoard = BoardUtilities.SerializeBoard(BoardUtilities.GenerateVisibleBoard(second.Item1));
                    game.State = context.GameStates.FirstOrDefault(s => s.State == InProgressState);
                    context.SaveChanges();

                    return "";
                }
            });

            return response;
        }

        private static Unit GetUnit(Game game, int playerNumber, char attackedCell)
        {
            Unit unit = null;
            switch (attackedCell)
            {
                case Cell.HitAircraftCarrierBody:
                    if (playerNumber == 1)
                    {
                        unit = game.SecondPlayerUnits.FirstOrDefault(u => u.Type.Type == Ship.AircraftCarrier);
                    }
                    else
                    {
                        unit = game.FirstPlayerUnits.FirstOrDefault(u => u.Type.Type == Ship.AircraftCarrier);
                    }
                    break;
                case Cell.HitBattleshipBody:
                    if (playerNumber == 1)
                    {
                        unit = game.SecondPlayerUnits.FirstOrDefault(u => u.Type.Type == Ship.Battleship);
                    }
                    else
                    {
                        unit = game.FirstPlayerUnits.FirstOrDefault(u => u.Type.Type == Ship.Battleship);
                    }
                    break;
                case Cell.HitDestroyerBody:
                    if (playerNumber == 1)
                    {
                        unit = game.SecondPlayerUnits.FirstOrDefault(u => u.Type.Type == Ship.Destroyer);
                    }
                    else
                    {
                        unit = game.FirstPlayerUnits.FirstOrDefault(u => u.Type.Type == Ship.Destroyer);
                    }
                    break;
                case Cell.HitSubmarineBody:
                    if (playerNumber == 1)
                    {
                        unit = game.SecondPlayerUnits.FirstOrDefault(u => u.Type.Type == Ship.Submarine);
                    }
                    else
                    {
                        unit = game.FirstPlayerUnits.FirstOrDefault(u => u.Type.Type == Ship.Submarine);
                    }
                    break;
                case Cell.HitPatrolBoatBody:
                    if (playerNumber == 1)
                    {
                        unit = game.SecondPlayerUnits.FirstOrDefault(u => u.Type.Type == Ship.PatrolBoat);
                    }
                    else
                    {
                        unit = game.FirstPlayerUnits.FirstOrDefault(u => u.Type.Type == Ship.PatrolBoat);
                    }
                    break;
                default:
                    throw new ServerErrorException("Invalid ship body.", ErrorType.GeneralError);
            }
            return unit;
        }

        private static GameWithBoardsViewModel GetGameModel(Game game, int playerNumber)
        {
            var firstPlayerUnits = game.FirstPlayerUnits
                .Where(u => u.State.State != DestroyedState)
                .Select(u => u.Type.Type);
            var secondPlayerUnits = game.SecondPlayerUnits
                .Where(u => u.State.State != DestroyedState)
                .Select(u => u.Type.Type);
            return new[] { game }.Select(g => new GameWithBoardsViewModel()
            {
                Id = g.Id,
                Title = g.Title,
                FirstPlayer = g.FirstPlayer != null ? g.FirstPlayer.Username : "",
                SecondPlayer = g.SecondPlayer != null ? g.SecondPlayer.Username : "",
                State = g.State.State,
                MyBoard = ((playerNumber == 1) ? g.FirstPlayerBoard : g.SecondPlayerBoard),
                OpponentBoard = ((playerNumber == 1) ? g.SecondPlayerVisibleBoard : g.FirstPlayerVisibleBoard),
                MyUnits = ((playerNumber == 1) ? firstPlayerUnits : secondPlayerUnits),
                OpponentUnits = ((playerNumber == 1) ? secondPlayerUnits : firstPlayerUnits),
                Winner = game.Winner != null ? game.Winner.Username : ""
            }).First();
        }

        private int CheckWinner(Game game, int playerNumber)
        {
            if (playerNumber == 1)
            {
                var remainingUnits = game.SecondPlayerUnits.Where(u => u.State.State != DestroyedState);
                if (remainingUnits.Count() == 0)
                {
                    game.Winner = game.FirstPlayer;
                    return 1;
                }
            }
            else
            {
                var remainingUnits = game.FirstPlayerUnits.Where(u => u.State.State != DestroyedState);
                if (remainingUnits.Count() == 0)
                {
                    game.Winner = game.SecondPlayer;
                    return 2;
                }
            }

            return 0;
        }
    }
}
