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
                        units.Add(new Unit()
                        {
                            User = playerNumber == 1 ? game.FirstPlayer : game.SecondPlayer,
                            Type = type,
                            State = state,
                            Row = model.Row,
                            Col = model.Col,
                            Rotation = rotation,
                            Game = game
                        });
                    }

                    if (playerNumber == 1)
                    {
                        game.FirstPlayerBoard = new Board() { BoardBody = BoardUtilities.SerializeBoard(board) };
                        game.FirstPlayerUnits = units;
                    }
                    else if (playerNumber == 2)
                    {
                        game.SecondPlayerBoard = new Board() { BoardBody = BoardUtilities.SerializeBoard(board) };
                        game.SecondPlayerUnits = units;
                    }

                    game.FirstPlayerVisibleBoard = new Board() { BoardBody = BoardUtilities.SerializeBoard(BoardUtilities.GenerateEmptyBoard()) };
                    game.SecondPlayerVisibleBoard = new Board() { BoardBody = BoardUtilities.SerializeBoard(BoardUtilities.GenerateEmptyBoard()) };

                    context.SaveChanges();
                    game = context.Games.FirstOrDefault(g => g.Id == id);
                    if (game.FirstPlayerBoard != null && game.SecondPlayerBoard != null)
                    {
                        var state = context.GameStates.First(s => s.State == GameReadyState);
                        game.State = state;
                    }

                    context.SaveChanges();

                    return new[] { game }.Select(g => new GameWithBoardsViewModel()
                    {
                        Id = g.Id,
                        Title = g.Title,
                        FirstPlayer = g.FirstPlayer.Username,
                        SecondPlayer = g.SecondPlayer.Username,
                        State = g.State.State,
                        MyBoard = ((playerNumber == 1) ? g.FirstPlayerBoard.BoardBody : g.SecondPlayerBoard.BoardBody),
                        OpponentBoard = ((playerNumber == 1) ? g.SecondPlayerVisibleBoard.BoardBody : g.FirstPlayerVisibleBoard.BoardBody),
                    }).First();
                }
            });

            return response;
        }
    }
}
