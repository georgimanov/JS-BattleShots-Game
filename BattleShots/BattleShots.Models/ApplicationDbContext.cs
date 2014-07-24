using System;
using System.Data.Entity;
using System.Linq;

namespace BattleShots.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageType> MessageTypes { get; set; }
        public DbSet<MessageState> MessageStates { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Rotation> Rotations { get; set; }
        public DbSet<UnitState> UnitStates { get; set; }
        public DbSet<UnitType> UnitTypes { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameState> GameStates { get; set; }
    }
}
