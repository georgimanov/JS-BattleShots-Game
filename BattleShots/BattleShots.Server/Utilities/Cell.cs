using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShots.Server.Utilities
{
    public class Cell
    {
        public const char AircraftCarrierBody = 'A';
        public const char BattleshipBody = 'B';
        public const char SubmarineBody = 'S';
        public const char DestroyerBody = 'D';
        public const char PatrolBoatBody = 'P';

        public const char Empty = '0';
        public const char HitWaterBody = '~';
        public const char HitShipBody = '*';
    }
}