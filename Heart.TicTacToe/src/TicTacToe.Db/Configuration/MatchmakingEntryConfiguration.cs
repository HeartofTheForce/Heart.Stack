using TicTacToe.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicTacToe.Db.Configuration
{
    public class MatchmakingEntryConfiguration : IEntityTypeConfiguration<MatchmakingEntry>
    {
        public void Configure(EntityTypeBuilder<MatchmakingEntry> builder)
        {
            builder.ToTable("MatchmakingEntries");

            builder.HasKey(x => x.PlayerId);
        }
    }
}
