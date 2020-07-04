using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Logic.Users;
using TicTacToe.Api.Models.Auth;
using TicTacToe.Api.Models.Users;

namespace TicTacToe.Api.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IUserManager _userManager;

        public AuthController(
            IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("api/register")]
        public async Task<ActionResult<UserModel>> Register([FromBody] RegisterInputModel inputModel)
        {
            if (InvalidModelState(out var invalidResponse))
                return invalidResponse;

            var reasonResult = await _userManager.RegisterAsync(inputModel);
            return BuildResponse(reasonResult);
        }

        [HttpPost("api/login")]
        public async Task<ActionResult<AuthResponse>> LoginPassword([FromBody] LoginPasswordModel inputModel)
        {
            if (InvalidModelState(out var invalidResponse))
                return invalidResponse;

            var reasonResult = await _userManager.LoginWithPasswordAsync(inputModel);
            return BuildResponse(reasonResult);
        }

        [HttpPost("api/login-refresh")]
        public async Task<ActionResult<AuthResponse>> LoginRefreshToken([FromBody] LoginRefreshTokenModel inputModel)
        {
            if (InvalidModelState(out var invalidResponse))
                return invalidResponse;

            var reasonResult = await _userManager.LoginWithRefreshTokenAsync(inputModel);
            return BuildResponse(reasonResult);
        }
    }
}
