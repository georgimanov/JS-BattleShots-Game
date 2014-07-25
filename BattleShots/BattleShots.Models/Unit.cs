using System;
using System.ComponentModel.DataAnnotations;

namespace BattleShots.Models
{
    public class Unit
    {
        public int Id { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public virtual Rotation Rotation { get; set; }

        [Required]
        public string Body { get; set; }

        public virtual UnitType Type { get; set; }

        public virtual UnitState State { get; set; }

        public virtual User User { get; set; }

        public virtual Game Game { get; set; }
    }
}
