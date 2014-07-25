using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BattleShots.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required, StringLength(50, MinimumLength = 2, ErrorMessage = "The game title must be between {2} and {1} characters long.")]
        public string Title { get; set; }

        public string Password { get; set; }

        public virtual User FirstPlayer { get; set; }

        public virtual User SecondPlayer { get; set; }

        public virtual GameState State { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual Board FirstPlayerBoard { get; set; }

        public virtual Board SecondPlayerBoard { get; set; }
    }
}
