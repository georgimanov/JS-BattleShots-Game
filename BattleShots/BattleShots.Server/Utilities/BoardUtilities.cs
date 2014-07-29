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
        private const int BoardSize = 10;

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

        private static int GetLength(string unitModelType)
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
    }
}