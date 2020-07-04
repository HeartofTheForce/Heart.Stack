using System;
using System.Threading.Tasks;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Auth;
using TicTacToe.Api.Models.Users;

namespace TicTacToe.Api.Logic.Users
{
    public interface IUserManager
    {
        Task<ReasonResult<UserModel>> RegisterAsync(RegisterInputModel inputModel);
        Task<ReasonResult<AuthResponse>> LoginWithPasswordAsync(LoginPasswordModel inputModel);
        Task<ReasonResult<AuthResponse>> LoginWithRefreshTokenAsync(LoginRefreshTokenModel inputModel);
    }
}
