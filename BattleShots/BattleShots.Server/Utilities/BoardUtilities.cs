using BattleShots.Models;
using BattleShots.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BattleShots.Server.Utilities
{
    public static class BoardUtilities
    {
        private static Random random = new Random();

        private const int BoardSize = 10;

        public static char[,] GenerateVisibleBoard(char[,] attackedBoard)
        {
            char[,] visibleBoard = new char[BoardSize, BoardSize];
            for (int i = 0; i < attackedBoard.GetLength(0); i++)
            {
                for (int j = 0; j < attackedBoard.GetLength(1); j++)
                {
                    if (attackedBoard[i, j] == Cell.HitWaterBody)
                    {
                        visibleBoard[i, j] = Cell.HitWaterBody;
                    }
                    else if (attackedBoard[i, j] == Cell.HitAircraftCarrierBody ||
                        attackedBoard[i, j] == Cell.HitBattleshipBody ||
                        attackedBoard[i, j] == Cell.HitDestroyerBody ||
                        attackedBoard[i, j] == Cell.HitSubmarineBody ||
                        attackedBoard[i, j] == Cell.HitPatrolBoatBody)
                    {
                        visibleBoard[i, j] = Cell.HitShipBody;
                    }
                    else
                    {
                        visibleBoard[i, j] = Cell.Empty;
                    }
                }
            }

            return visibleBoard;
        }

        public static char[,] GenerateEmptyBoard()
        {
            char[,] board = new char[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    board[i, j] = Cell.Empty;
                }
            }

            return board;
        }

        public static char[,] PlaceUnit(char[,] board, UnitBindingModel unitModel)
        {
            int length = GetLength(unitModel.UnitType);
            EnsureUnitIsInField(unitModel, length);
            EnsureUnitDoesNotOverlap(unitModel, board, length);
            if (unitModel.Rotation == Ship.HorizontalRotation)
            {
                for (int i = unitModel.Col - 1; i < unitModel.Col - 1 + length; i++)
                {
                    board[unitModel.Row - 1, i] = GetShipBody(unitModel.UnitType);
                }
            }
            else if (unitModel.Rotation == Ship.VerticalRotation)
            {
                for (int i = unitModel.Row - 1; i < unitModel.Row - 1 + length; i++)
                {
                    board[i, unitModel.Col - 1] = GetShipBody(unitModel.UnitType);
                }
            }

            return board;
        }

        public static string SerializeBoard(char[,] board)
        {
            StringBuilder boardBuilder = new StringBuilder();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    boardBuilder.Append(board[i, j]);
                }
            }

            return boardBuilder.ToString();
        }

        public static char[,] DeserializeBoard(string board)
        {
            char[,] deserializedBoard = new char[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    deserializedBoard[i, j] = board[i * BoardSize + j];
                }
            }

            return deserializedBoard;
        }

        internal static int GetLength(string unitModelType)
        {
            switch (unitModelType)
            {
                case Ship.AircraftCarrier:
                    return Ship.AircraftCarrierLength;
                case Ship.Battleship:
                    return Ship.BattleshipLength;
                case Ship.Submarine:
                    return Ship.SubmarineLength;
                case Ship.Destroyer:
                    return Ship.DestroyerLength;
                case Ship.PatrolBoat:
                    return Ship.PatrolBoatLength;
                default:
                    throw new ServerErrorException("Invalid unit.", ErrorType.InvalidInput);
            }
        }

        private static char GetShipBody(string unitModelType)
        {
            switch (unitModelType)
            {
                case Ship.AircraftCarrier:
                    return Cell.AircraftCarrierBody;
                case Ship.Battleship:
                    return Cell.BattleshipBody;
                case Ship.Submarine:
                    return Cell.SubmarineBody;
                case Ship.Destroyer:
                    return Cell.DestroyerBody;
                case Ship.PatrolBoat:
                    return Cell.PatrolBoatBody;
                default:
                    throw new ServerErrorException("Invalid unit.", ErrorType.InvalidInput);
            }
        }

        private static void EnsureUnitIsInField(UnitBindingModel unitModel, int length)
        {
            if (unitModel.Row < 1 || unitModel.Col < 1)
            {
                throw new ServerErrorException(
                    String.Format("Invalid position of unit at ({0}; {1})", unitModel.Row, unitModel.Col),
                    ErrorType.InvalidInput);
            }

            if (unitModel.Rotation != Ship.HorizontalRotation && unitModel.Rotation != Ship.VerticalRotation)
            {
                throw new ServerErrorException(
                    "Invalid rotation of unit.",
                    ErrorType.InvalidInput);
            }

            if (unitModel.Rotation == Ship.HorizontalRotation &&
                unitModel.Col + length - 1 > BoardSize)
            {
                throw new ServerErrorException(
                    String.Format("Invalid position of unit at ({0}; {1})", unitModel.Row, unitModel.Col),
                    ErrorType.InvalidInput);
            }
            else if (unitModel.Rotation == Ship.VerticalRotation &&
                unitModel.Row + length - 1 > BoardSize)
            {
                throw new ServerErrorException(
                    String.Format("Invalid position of unit at ({0}; {1})", unitModel.Row, unitModel.Col),
                    ErrorType.InvalidInput);
            }
        }

        private static void EnsureUnitDoesNotOverlap(UnitBindingModel unitModel, char[,] board, int length)
        {
            if (unitModel.Rotation == Ship.HorizontalRotation)
            {
                for (int i = unitModel.Col - 1; i < unitModel.Col - 1 + length; i++)
                {
                    if (board[unitModel.Row - 1, i] != Cell.Empty)
                    {
                        throw new ServerErrorException(
                            String.Format("Invalid position of unit at ({0}; {1})", unitModel.Row, unitModel.Col),
                            ErrorType.InvalidInput);
                    }
                }
            }
            else if (unitModel.Rotation == Ship.VerticalRotation)
            {
                for (int i = unitModel.Row - 1; i < unitModel.Row - 1 + length; i++)
                {
                    if (board[i, unitModel.Col - 1] != Cell.Empty)
                    {
                        throw new ServerErrorException(
                            String.Format("Invalid position of unit at ({0}; {1})", unitModel.Row, unitModel.Col),
                            ErrorType.InvalidInput);
                    }
                }
            }
        }

        private static bool EnsureRandomUnitIsInField(UnitBindingModel unitModel, int length)
        {
            if (unitModel.Row < 1 || unitModel.Col < 1)
            {
                return false;
            }

            if (unitModel.Rotation != Ship.HorizontalRotation && unitModel.Rotation != Ship.VerticalRotation)
            {
                return false;
            }

            if (unitModel.Rotation == Ship.HorizontalRotation &&
                unitModel.Col + length - 1 > BoardSize)
            {
                return false;
            }
            else if (unitModel.Rotation == Ship.VerticalRotation &&
                unitModel.Row + length - 1 > BoardSize)
            {
                return false;
            }

            return true;
        }

        private static bool EnsureRandomUnitDoesNotOverlap(UnitBindingModel unitModel, char[,] board, int length)
        {
            if (unitModel.Rotation == Ship.HorizontalRotation)
            {
                for (int i = unitModel.Col - 1; i < unitModel.Col - 1 + length; i++)
                {
                    if (board[unitModel.Row - 1, i] != Cell.Empty)
                    {
                        return false;
                    }
                }

                return true;
            }
            else if (unitModel.Rotation == Ship.VerticalRotation)
            {
                for (int i = unitModel.Row - 1; i < unitModel.Row - 1 + length; i++)
                {
                    if (board[i, unitModel.Col - 1] != Cell.Empty)
                    {
                        return false;
                    }
                }

                return true;
            }

            throw new InvalidOperationException("Invalid rotation");
        }

        public static void ValidateAttack(AttackingModel model)
        {
            if (model.Row < 1 || model.Row > BoardSize)
            {
                throw new ServerErrorException("The specified row is invalid.", ErrorType.InvalidInput);
            }

            if (model.Col < 1 || model.Col > BoardSize)
            {
                throw new ServerErrorException("The specified column is invalid.", ErrorType.InvalidInput);
            }
        }

        public static char Attack(char[,] attackedBoard, AttackingModel attackingModel)
        {
            char cell = attackedBoard[attackingModel.Row - 1, attackingModel.Col - 1];
            if (cell == Cell.Empty || cell == Cell.HitWaterBody)
            {
                cell = Cell.HitWaterBody;
            }
            else if (cell == Cell.AircraftCarrierBody ||
                cell == Cell.BattleshipBody ||
                cell == Cell.DestroyerBody ||
                cell == Cell.SubmarineBody ||
                cell == Cell.PatrolBoatBody)
            {
                switch (cell)
                {
                    case Cell.AircraftCarrierBody:
                        cell = Cell.HitAircraftCarrierBody;
                        break;
                    case Cell.BattleshipBody:
                        cell = Cell.HitBattleshipBody;
                        break;
                    case Cell.DestroyerBody:
                        cell = Cell.HitDestroyerBody;
                        break;
                    case Cell.SubmarineBody:
                        cell = Cell.HitSubmarineBody;
                        break;
                    case Cell.PatrolBoatBody:
                        cell = Cell.HitPatrolBoatBody;
                        break;
                }
            }
            else
            {
                return '\0';
            }

            return cell;
        }

        internal static Tuple<char[,], List<UnitBindingModel>> GenerateRandomBoard()
        {
            char[,] board = BoardUtilities.GenerateEmptyBoard();
            var units = new[] { Ship.AircraftCarrier, Ship.Battleship, Ship.Destroyer, Ship.Submarine, Ship.PatrolBoat };
            var generatedUnits = new List<UnitBindingModel>(units.Length);
            var rotations = new[] { Ship.HorizontalRotation, Ship.VerticalRotation };
            for (int i = 0; i < units.Length; i++)
            {
                int randomRow = random.Next(1, 11);
                int randomCol = random.Next(1, 11);
                string randomRotation = rotations[random.Next(0, 2)];
                var model = new UnitBindingModel() { Row = randomRow, Col = randomCol, Rotation = randomRotation, UnitType = units[i] };
                while (!EnsureRandomUnitIsInField(model, GetLength(model.UnitType)) ||
                    !EnsureRandomUnitDoesNotOverlap(model, board, GetLength(model.UnitType)))
                {
                    randomRow = random.Next(1, 11);
                    randomCol = random.Next(1, 11);
                    randomRotation = rotations[random.Next(0, 2)];
                    model = new UnitBindingModel() { Row = randomRow, Col = randomCol, Rotation = randomRotation, UnitType = units[i] };
                }

                generatedUnits.Add(model);
                board = PlaceUnit(board, model);
            }

            return new Tuple<char[,], List<UnitBindingModel>>(board, generatedUnits);
        }
    }
}