using System;
using System.Threading;
using System.Threading.Tasks;
using TicTacToe.Api.Logic.Games;
using TicTacToe.Api.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TicTacToe.Api.Logic.HostedServices
{
    public class MatchmakingService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TicTacToeSettings _ticTacToeSettings;
        private readonly ILogger<MatchmakingService> _logger;

        private readonly Timer _timer;

        public MatchmakingService(
            IServiceProvider serviceProvider,
            IOptions<TicTacToeSettings> options,
            ILogger<MatchmakingService> logger)
        {
            _serviceProvider = serviceProvider;
            _ticTacToeSettings = options.Value;
            _logger = logger;

            _timer = new Timer(Handle);
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MatchmakingService is starting");

            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_ticTacToeSettings.MatchmakingIntervalInSeconds));

            return Task.CompletedTask;
        }

        private async void Handle(object? state)
        {
            _logger.LogInformation("MatchmakingService is executing");

            using var scope = _serviceProvider.CreateScope();
            var matchmakingManager = scope.ServiceProvider.GetRequiredService<IMatchmakingManager>();
            await matchmakingManager.CleanupStaleMatchmakingEntries();
            await matchmakingManager.MatchPlayersInQueue();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MatchmakingService is stopping");

            _timer.Dispose();

            return Task.CompletedTask;
        }
    }
}
