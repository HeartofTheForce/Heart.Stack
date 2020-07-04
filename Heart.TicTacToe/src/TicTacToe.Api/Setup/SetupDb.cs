using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Api.Settings;
using TicTacToe.Db;

namespace TicTacToe.Api.Setup
{
    public static partial class SetupUtils
    {
        public static IServiceCollection ConfigureDb(
            this IServiceCollection services,
            TicTacToeSettings settings)
        {
            services
                .AddDbContext<TicTacToeDbContext>(options => options.UseSqlServer(settings.ConnectionString));

            return services;
        }
    }
}
