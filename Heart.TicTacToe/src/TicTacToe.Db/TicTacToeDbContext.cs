using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Db.Configuration;
using TicTacToe.Db.Models;

namespace TicTacToe.Db
{
    public class TicTacToeDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = default!;
        public DbSet<Game> Games { get; set; } = default!;
        public DbSet<MatchmakingEntry> MatchmakingEntries { get; set; } = default!;

        public TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ApplicationUserConfig());
            modelBuilder.ApplyConfiguration(new GameConfiguration());
            modelBuilder.ApplyConfiguration(new MatchmakingEntryConfiguration());
        }
    }
}
