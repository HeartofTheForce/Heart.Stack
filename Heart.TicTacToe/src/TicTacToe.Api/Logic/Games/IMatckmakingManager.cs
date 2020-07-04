using System;
using System.Threading.Tasks;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Games;

namespace TicTacToe.Api.Logic.Games
{
    public interface IMatchmakingManager
    {
        Task<ReasonResult<QueueModel>> TryJoinQueue(ClaimsUser user);
        Task<DateTimeOffset> AddPlayerToQueue(string playerId);
        Task MatchPlayersInQueue();
        Task CleanupStaleMatchmakingEntries();
    }
}
