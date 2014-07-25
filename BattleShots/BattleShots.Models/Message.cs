using System;

namespace BattleShots.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public virtual Game Game { get; set; }

        public virtual User User { get; set; }

        public virtual MessageType Type { get; set; }

        public virtual MessageState State { get; set; }
    }
}
