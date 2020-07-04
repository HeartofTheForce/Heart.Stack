using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicTacToe.Db.Models;

namespace TicTacToe.Db.Configuration
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
        }
    }
}
