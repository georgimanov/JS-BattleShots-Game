using System;
using System.ComponentModel.DataAnnotations;

namespace BattleShots.Models
{
    public class Unit
    {
        public int Id { get; set; }

        [Required]
        public int Row { get; set; }

        [Required]
        public int Col { get; set; }

        [Required]
        public int RemainingCells { get; set; }

        public virtual Rotation Rotation { get; set; }

        public virtual UnitType Type { get; set; }

        public virtual UnitState State { get; set; }

        public virtual User User { get; set; }

        public virtual Game Game { get; set; }
    }
}
