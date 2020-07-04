using System.Threading.Tasks;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Games;

namespace TicTacToe.Api.Logic.Games
{
    public interface IGameManager
    {
        Task<ReasonResult<GameModel>> GetGameByIdAsync(int id, ClaimsUser user);
        Task<ReasonResult<GameModel>> TakeTurnAsync(int id, int turnX, int turnY, ClaimsUser user);
        Task CleanupStaleGames();
    }
}
