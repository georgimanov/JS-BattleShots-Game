using System;
using System.ComponentModel.DataAnnotations;

namespace BattleShots.Models
{
    public class Board
    {
        public int Id { get; set; }

        [Required]
        public string BoardBody { get; set; }
    }
}
