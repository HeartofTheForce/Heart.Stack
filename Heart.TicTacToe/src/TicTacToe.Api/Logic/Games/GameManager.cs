using System;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.Api.Settings;
using TicTacToe.Db;
using TicTacToe.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TicTacToe.Api.Logic.BoardState;
using TicTacToe.Api.Models.Games;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Logic.Games
{
    public class GameManager : IGameManager
    {
        private static Reason GameInWrongState => new Reason("GameInWrongState", "Game in wrong State");

        private static Reason TileAlreadySet => new Reason("TileAlreadySet", "Tile already set");
        private static Reason ActingOnWrongTurn => new Reason("ActingOnWrongTurn", "Acting on wrong turn");

        private readonly TicTacToeDbContext _dbContext;
        private readonly TicTacToeSettings _ticTacToeSettings;
        private readonly ILogger<GameManager> _logger;

        public GameManager(
            TicTacToeDbContext dbContext,
            IOptions<TicTacToeSettings> options,
            ILogger<GameManager> logger)
        {
            _dbContext = dbContext;
            _ticTacToeSettings = options.Value;
            _logger = logger;
        }

        public async Task<ReasonResult<GameModel>> GetGameByIdAsync(int id, ClaimsUser user)
        {
            var game = await _dbContext.Games.FirstOrDefaultAsync(x => x.Id == id);

            if (game == null)
                return ReasonResult<GameModel>.NotFound();

            if (game.PlayerOne != user.Id && game.PlayerTwo != user.Id)
                return ReasonResult<GameModel>.NotFound();

            return ReasonResult<GameModel>.Success(new GameModel(game));
        }


        public async Task<ReasonResult<GameModel>> TakeTurnAsync(int id, int turnX, int turnY, ClaimsUser user)
        {
            var game = await _dbContext.Games
                .FirstOrDefaultAsync(x => x.Id == id);

            if (game == null)
                return ReasonResult<GameModel>.NotFound();

            BoardPlayer player;
            if (game.PlayerOne == user.Id)
                player = BoardPlayer.PlayerOne;
            else if (game.PlayerTwo == user.Id)
                player = BoardPlayer.PlayerTwo;
            else
                return ReasonResult<GameModel>.NotFound();

            if (game.GameState != GameState.InProgress)
            {
                _logger.LogInformation("Player tried to take turn on game with incorrect state, {@details}", new
                {
                    GameId = game.Id,
                    GameState = game.GameState,
                    UserId = user.Id,
                });

                return ReasonResult<GameModel>.BadRequest(new[] { GameInWrongState });
            }

            var oldDecompressedBoardState = new DecompressedBoardState(game.BoardHistory);
            var takeTurnResult = oldDecompressedBoardState.TryTakeTurn(turnX, turnY, player, out var newDecompressedBoardState);

            switch (takeTurnResult)
            {
                case TakeTurnResult.Success:
                    {
                        _logger.LogInformation("Player took turn, {@details}", new
                        {
                            GameId = game.Id,
                            UserId = user.Id,
                            GameState = game.GameState,
                            X = turnX,
                            Y = turnY,
                            OldBoardHistory = oldDecompressedBoardState.BoardHistory,
                            NewBoardHistory = newDecompressedBoardState.BoardHistory,
                            Snapshot = newDecompressedBoardState.ToString(),
                        });

                        UpdateGameUsingDecompressedBoardState(game, newDecompressedBoardState);

                        await _dbContext.SaveChangesAsync();
                        return ReasonResult<GameModel>.Success(new GameModel(game));
                    }
                case TakeTurnResult.NotInProgress:
                    return ReasonResult<GameModel>.BadRequest(new[] { GameInWrongState });
                case TakeTurnResult.TileAlreadySet:
                    return ReasonResult<GameModel>.BadRequest(new[] { TileAlreadySet });
                case TakeTurnResult.WrongPlayer:
                    return ReasonResult<GameModel>.BadRequest(new[] { ActingOnWrongTurn });
                default:
                    throw new Exception($"Unknown failure state reached during {nameof(DecompressedBoardState)}.{nameof(DecompressedBoardState.TryTakeTurn)}");
            }
        }

        private void UpdateGameUsingDecompressedBoardState(Game game, DecompressedBoardState decompressedBoardState)
        {
            game.BoardHistory = decompressedBoardState.BoardHistory;
            game.LastActionDate = DateTime.UtcNow;

            switch (decompressedBoardState.BoardResult)
            {
                case BoardResult.InProgress:
                    {
                        game.GameResult = null;
                        game.GameState = GameState.InProgress;
                    }
                    break;
                case BoardResult.WinPlayerOne:
                    {
                        game.GameResult = GameResult.WinPlayerOne;
                        game.GameState = GameState.Completed;

                        _logger.LogInformation("Game Completed, {@details}", new
                        {
                            GameId = game.Id,
                            GameResult = game.GameResult,
                            Winner = game.PlayerOne,
                            Turn = decompressedBoardState.CurrentTurn,
                            BoardHistory = decompressedBoardState.BoardHistory,
                            Snapshot = decompressedBoardState.ToString(),
                        });
                    }
                    break;
                case BoardResult.WinPlayerTwo:
                    {
                        game.GameResult = GameResult.WinPlayerTwo;
                        game.GameState = GameState.Completed;

                        _logger.LogInformation("Game Completed, {@details}", new
                        {
                            GameId = game.Id,
                            GameResult = game.GameResult,
                            Winner = game.PlayerTwo,
                            Turn = decompressedBoardState.CurrentTurn,
                            BoardHistory = decompressedBoardState.BoardHistory,
                            Snapshot = decompressedBoardState.ToString(),
                        });
                    }
                    break;
                case BoardResult.Draw:
                    {
                        game.GameResult = GameResult.Draw;
                        game.GameState = GameState.Completed;

                        _logger.LogInformation("Game Completed, {@details}", new
                        {
                            GameId = game.Id,
                            GameResult = game.GameResult,
                            Turn = decompressedBoardState.CurrentTurn,
                            Snapshot = decompressedBoardState.ToString(),
                        });
                    }
                    break;
            }
        }

        public async Task CleanupStaleGames()
        {
            var staleGames = await _dbContext.Games
                .Where(x =>
                    x.GameState == GameState.InProgress &&
                     x.LastActionDate < DateTime.UtcNow.AddMinutes(-_ticTacToeSettings.StaleInMinutes))
                .ToListAsync();

            foreach (var game in staleGames)
            {
                game.GameState = GameState.Aborted;
                var decompressedBoardState = new DecompressedBoardState(game.BoardHistory);
                game.GameResult = decompressedBoardState.CurrentTurn % 2 == 0 ? GameResult.WinPlayerTwo : GameResult.WinPlayerOne;

                _logger.LogInformation("Player forfeited due to inaction, {@details}", new
                {
                    GameId = game.Id,
                    StaleInMinutes = _ticTacToeSettings.StaleInMinutes
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
