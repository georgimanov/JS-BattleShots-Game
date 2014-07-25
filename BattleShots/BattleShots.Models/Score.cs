using System;

namespace BattleShots.Models
{
    public class Score
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public virtual User User { get; set; }
    }
}
