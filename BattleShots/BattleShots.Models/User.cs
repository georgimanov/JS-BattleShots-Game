using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BattleShots.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(40, MinimumLength = 6, ErrorMessage = "The username must be between {2} and {1} characters long.")]
        public string Username { get; set; }

        public string Password { get; set; }
        
        public string SessionKey { get; set; }

        [Required]
        public int Score { get; set; }

        public virtual List<Game> GamesCreated { get; set; }

        public virtual List<Game> GamesJoined { get; set; }

        public virtual List<Message> Messages { get; set; }
    }
}
