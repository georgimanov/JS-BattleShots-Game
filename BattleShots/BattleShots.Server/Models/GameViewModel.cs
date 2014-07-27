using BattleShots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BattleShots.Server.Models
{
    public class GameViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string FirstPlayer { get; set; }

        public string SecondPlayer { get; set; }

        public string State { get; set; }

        public static Func<Game, GameViewModel> FromGame
        {
            get
            {
                return g => new GameViewModel()
                {
                    Id = g.Id,
                    Title = g.Title,
                    FirstPlayer = g.FirstPlayer.Username,
                    SecondPlayer = g.SecondPlayer == null ? "" : g.SecondPlayer.Username,
                    State = g.State.State
                };
            }
        }
    }
}