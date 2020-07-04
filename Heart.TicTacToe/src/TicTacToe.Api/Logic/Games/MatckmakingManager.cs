using System;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.Api.Settings;
using TicTacToe.Db;
using TicTacToe.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Games;

namespace TicTacToe.Api.Logic.Games
{
    public class MatchmakingManager : IMatchmakingManager
    {
        private static readonly Random s_random = new Random();
        private static readonly object s_randLock = new object();

        private readonly TicTacToeDbContext _dbContext;
        private readonly TicTacToeSettings _ticTacToeSettings;
        private readonly ILogger<MatchmakingManager> _logger;

        public MatchmakingManager(
            TicTacToeDbContext dbContext,
            IOptions<TicTacToeSettings> options,
            ILogger<MatchmakingManager> logger)
        {
            _dbContext = dbContext;
            _ticTacToeSettings = options.Value;
            _logger = logger;
        }

        public async Task<ReasonResult<QueueModel>> TryJoinQueue(ClaimsUser user)
        {
            var currentGame = await _dbContext.Games
                .FirstOrDefaultAsync(x =>
                    x.GameState == GameState.InProgress &&
                    (x.PlayerOne == user.Id || x.PlayerTwo == user.Id));

            QueueModel output;
            if (currentGame == null)
            {
                output = new QueueModel()
                {
                    InQueueSince = await AddPlayerToQueue(user.Id),
                };
            }
            else
            {
                _logger.LogInformation("Player tried to join game while already in a game, {@details}", new
                {
                    GameId = currentGame.Id,
                    UserId = user.Id
                });

                output = new QueueModel()
                {
                    GameId = currentGame.Id,
                };
            }

            return ReasonResult<QueueModel>.Success(output);
        }

        public async Task<DateTimeOffset> AddPlayerToQueue(string playerId)
        {
            var currentEntry = await _dbContext.MatchmakingEntries.FirstOrDefaultAsync(x => x.PlayerId == playerId);

            if (currentEntry == null)
            {
                currentEntry = new MatchmakingEntry(playerId, DateTime.UtcNow);
                _dbContext.MatchmakingEntries.Add(currentEntry);

                _logger.LogInformation("Player joined queue, {@details}", new
                {
                    PlayerId = playerId,
                });
            }
            else
            {
                currentEntry.LatestJoinDate = DateTime.UtcNow;

                _logger.LogInformation("Player rejoined queue, {@details}", new
                {
                    PlayerId = playerId,
                });
            }

            await _dbContext.SaveChangesAsync();

            return currentEntry.CreatedDate;
        }

        public async Task MatchPlayersInQueue()
        {
            var availablePlayers = await _dbContext.MatchmakingEntries
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();

            var newGames = new List<Game>();
            var entriesToRemove = new List<MatchmakingEntry>();
            for (int i = 0; i < availablePlayers.Count / 2; i++)
            {
                newGames.Add(CreateGame(availablePlayers[i].PlayerId, availablePlayers[i + 1].PlayerId));
                entriesToRemove.Add(availablePlayers[i]);
                entriesToRemove.Add(availablePlayers[i + 1]);
            }

            _dbContext.Games.AddRange(newGames);
            _dbContext.MatchmakingEntries.RemoveRange(entriesToRemove);
            await _dbContext.SaveChangesAsync();

            for (int i = 0; i < newGames.Count; i++)
            {
                _logger.LogInformation("Created new game, {@details}", new
                {
                    GameId = newGames[i].Id,
                    PlayerOne = newGames[i].PlayerOne,
                    PlayerTwo = newGames[i].PlayerTwo
                });
            }

            for (int i = 0; i < entriesToRemove.Count; i++)
            {
                _logger.LogInformation("Player left queue, {@details}", new
                {
                    PlayerId = entriesToRemove[i].PlayerId,
                });
            }
        }

        private Game CreateGame(string playerOneId, string playerTwoId)
        {
            int randomNumber;
            lock (s_randLock)
            {
                randomNumber = s_random.Next(0, 2);
            }

            if (randomNumber > 0)
            {
                string swap = playerOneId;
                playerOneId = playerTwoId;
                playerTwoId = swap;
            }

            var game = new Game(
            playerOneId,
            playerTwoId,
            GameState.InProgress,
            DateTime.UtcNow);

            return game;
        }

        public async Task CleanupStaleMatchmakingEntries()
        {
            var staleEntries = await _dbContext.MatchmakingEntries
                .Where(x => x.LatestJoinDate < DateTime.UtcNow.AddMinutes(-_ticTacToeSettings.MatchmakingTimeoutInMinutes))
                .ToListAsync();

            if (staleEntries.Any())
            {
                _logger.LogInformation("Removing inactive players from queue, {@details}", new
                {
                    PlayerIds = staleEntries.Select(x => x.PlayerId),
                });

                _dbContext.MatchmakingEntries.RemoveRange(staleEntries);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
