using BattleShots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BattleShots.Server.Models
{
    public class GameWithBoardsViewModel : GameViewModel
    {
        public string MyBoard { get; set; }

        public string OpponentBoard { get; set; }

        public string Winner { get; set; }

        public IEnumerable<string> MyUnits { get; set; }

        public IEnumerable<string> OpponentUnits { get; set; }
    }
}