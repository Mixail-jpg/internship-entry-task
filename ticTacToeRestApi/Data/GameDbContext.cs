using Microsoft.EntityFrameworkCore;
using ticTacToeRestApi.Data.Entities;

namespace ticTacToeRestApi.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) 
            : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.PlayerX)
                .WithMany(p => p.GamesAsX)
                .HasForeignKey(g => g.PlayerXId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.PlayerO)
                .WithMany(p => p.GamesAsO)
                .HasForeignKey(g => g.PlayerOId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.Moves)
                .WithOne(m => m.Player)
                .HasForeignKey(m => m.PlayerId);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Moves)
                .WithOne(m => m.Game)
                .HasForeignKey(m => m.GameId);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Winner)
                .WithMany()
                .HasForeignKey(g => g.WinnerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Move>()
                .HasIndex(m => new { m.GameId, m.Row, m.Column })
                .IsUnique();
        }
    }

    
}
