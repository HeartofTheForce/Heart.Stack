using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Logic.Games;
using TicTacToe.Api.Models.Games;

namespace TicTacToe.Api.Controllers
{
    public class GameController : ControllerPlus
    {
        private readonly IGameManager _gameManager;
        private readonly IMatchmakingManager _matchmakingManager;

        public GameController(
            IGameManager gameManager,
            IMatchmakingManager matchmakingManager)
        {
            _gameManager = gameManager;
            _matchmakingManager = matchmakingManager;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok("Alive");
        }

        [Authorize]
        [HttpPost("api/game/join-queue")]
        public async Task<ActionResult<QueueModel>> TryJoinQueue()
        {
            if (InvalidModelState(out var invalidResponse))
                return invalidResponse;

            var reasonResult = await _matchmakingManager.TryJoinQueue(ClaimsUser);
            return BuildResponse(reasonResult);
        }

        [Authorize]
        [HttpPost("api/game/take-turn")]
        public async Task<ActionResult<GameModel>> TakeTurn(
            [FromQuery] int id,
            [FromQuery] int x,
            [FromQuery] int y)
        {
            if (InvalidModelState(out var invalidResponse))
                return invalidResponse;

            var reasonResult = await _gameManager.TakeTurnAsync(id, x, y, ClaimsUser);
            return BuildResponse(reasonResult);
        }

        [Authorize]
        [HttpGet("api/game")]
        public async Task<ActionResult<GameModel>> GetGame([FromQuery] int id)
        {
            if (InvalidModelState(out var invalidResponse))
                return invalidResponse;

            var reasonResult = await _gameManager.GetGameByIdAsync(id, ClaimsUser);
            return BuildResponse(reasonResult);
        }

    }
}
