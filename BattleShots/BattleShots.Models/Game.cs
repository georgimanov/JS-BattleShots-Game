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

        public int CurrentTurn { get; set; }

        public virtual User Winner { get; set; }

        public virtual GameState State { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<Unit> FirstPlayerUnits { get; set; }
        
        public virtual ICollection<Unit> SecondPlayerUnits { get; set; }

        public string FirstPlayerBoard { get; set; }
               
        public string FirstPlayerVisibleBoard { get; set; }
               
        public string SecondPlayerBoard { get; set; }
               
        public string SecondPlayerVisibleBoard { get; set; }

        public string FirstPlayerConnId { get; set; }

        public string SecondPlayerConnId { get; set; }
    }
}
