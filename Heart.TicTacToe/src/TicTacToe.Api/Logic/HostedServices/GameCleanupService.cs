using System;
using System.Threading;
using System.Threading.Tasks;
using TicTacToe.Api.Logic.Games;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TicTacToe.Api.Logic.HostedServices
{
    public class GameCleanupService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GameCleanupService> _logger;

        private readonly Timer _timer;

        public GameCleanupService(
            IServiceProvider serviceProvider,
            ILogger<GameCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            _timer = new Timer(Handle);
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CleanupService is starting");

            _timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private async void Handle(object? state)
        {
            _logger.LogInformation("CleanupService is executing");

            using var scope = _serviceProvider.CreateScope();
            var gameManager = scope.ServiceProvider.GetRequiredService<IGameManager>();
            await gameManager.CleanupStaleGames();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CleanupService is stopping");

            _timer.Dispose();

            return Task.CompletedTask;
        }
    }
}
