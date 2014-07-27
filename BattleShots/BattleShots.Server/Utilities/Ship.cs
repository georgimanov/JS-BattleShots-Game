using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShots.Server.Utilities
{
    public class Ship
    {
        public const string AircraftCarrier = "aircraft-carrier";
        public const string Battleship = "battleship";
        public const string Submarine = "submarine";
        public const string Destroyer = "destroyer";
        public const string PatrolBoat = "patrol-boat";
        
        public const int AircraftCarrierLength = 5;
        public const int BattleshipLength = 4;
        public const int SubmarineLength = 3;
        public const int DestroyerLength = 3;
        public const int PatrolBoatLength = 2;

        public const string FullState = "full";

        public const string HorizontalRotation = "horizontal";
        public const string VerticalRotation = "vertical";
    }
}