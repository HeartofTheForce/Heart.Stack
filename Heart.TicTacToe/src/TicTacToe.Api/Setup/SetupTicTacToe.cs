using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Api.Logic.Games;
using TicTacToe.Api.Logic.HostedServices;
using TicTacToe.Api.Settings;
using TicTacToe.Db;

namespace TicTacToe.Api.Setup
{
    public static partial class SetupUtils
    {
        public static IServiceCollection ConfigureTicTacToe(
            this IServiceCollection services)
        {
            services
                .AddTransient<IGameManager, GameManager>()
                .AddTransient<IMatchmakingManager, MatchmakingManager>()
                .AddHostedService<GameCleanupService>()
                .AddHostedService<MatchmakingService>()
            ;
            return services;
        }
    }
}
