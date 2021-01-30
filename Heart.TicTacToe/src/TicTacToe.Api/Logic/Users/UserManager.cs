using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TicTacToe.Api.Logic.Auth;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Auth;
using TicTacToe.Api.Models.Users;
using TicTacToe.Api.Settings;
using TicTacToe.Db;
using TicTacToe.Db.Models;

namespace TicTacToe.Api.Logic.Users
{
    public class UserManager : IUserManager
    {
        private readonly TicTacToeDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly ILogger<UserManager> _logger;

        public UserManager(
            TicTacToeDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtGenerator jwtGenerator,
            ILogger<UserManager> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
            _logger = logger;
        }

        public async Task<ReasonResult<UserModel>> RegisterAsync(RegisterInputModel inputModel)
        {
            var newUser = new ApplicationUser()
            {
                UserName = inputModel.Email,
                Email = inputModel.Email,
                Alias = inputModel.Alias,
                CreatedDate = DateTime.UtcNow,
            };

            var identityResult = await _userManager.CreateAsync(newUser, inputModel.Password);

            _logger.LogInformation("User Register, {@details}", new
            {
                Id = newUser.Id,
                UserName = newUser.UserName,
            });

            if (identityResult.Succeeded)
            {
                var output = new UserModel(newUser);
                return ReasonResult<UserModel>.Success(output);
            }
            else
            {
                return ReasonResult<UserModel>.BadRequest(identityResult.Errors.Select(x => new Reason(x.Code, x.Description)));
            }
        }

        public async Task<ReasonResult<AuthResponse>> LoginWithPasswordAsync(LoginPasswordModel inputModel)
        {
            var user = await _userManager.FindByEmailAsync(inputModel.Email);

            if (user != null)
            {
                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, inputModel.Password, lockoutOnFailure: true);

                if (signInResult.Succeeded)
                {
                    await _userManager.RemoveAuthenticationTokenAsync(user, AuthSettings.RefreshTokenProvider, AuthSettings.RefreshTokenName);
                    string newRefreshToken = await _userManager.GenerateUserTokenAsync(user, AuthSettings.RefreshTokenProvider, AuthSettings.RefreshTokenName);
                    await _userManager.SetAuthenticationTokenAsync(user, AuthSettings.RefreshTokenProvider, AuthSettings.RefreshTokenName, newRefreshToken);

                    string accessToken = _jwtGenerator.GenerateToken(user, out var expires);
                    var output = new AuthResponse(accessToken, newRefreshToken, expires);

                    return ReasonResult<AuthResponse>.Success(output);
                }
                else
                {
                    var reasons = new List<Reason>();

                    if (signInResult.IsLockedOut)
                        reasons.Add(new Reason("LockedOut", "User is locked out"));

                    if (signInResult.IsNotAllowed)
                        reasons.Add(new Reason("IsNotAllowed", "User is not allowed to sign in"));

                    if (!signInResult.IsLockedOut && !signInResult.IsNotAllowed && !signInResult.RequiresTwoFactor)
                        reasons.Add(new Reason("InvalidPassword", "Password does not match"));

                    return ReasonResult<AuthResponse>.BadRequest(reasons);
                }
            }

            return ReasonResult<AuthResponse>.BadRequest(new Reason[] { new Reason("UserNotFound", "Cannot find User") });
        }

        public async Task<ReasonResult<AuthResponse>> LoginWithRefreshTokenAsync(LoginRefreshTokenModel inputModel)
        {
            var query = (
                from userToken in _dbContext.UserTokens
                join applicationUser in _dbContext.ApplicationUsers on userToken.UserId equals applicationUser.Id
                where
                    userToken.LoginProvider == AuthSettings.RefreshTokenProvider &&
                    userToken.Name == AuthSettings.RefreshTokenName &&
                    userToken.Value == inputModel.RefreshToken
                select new
                {
                    ApplicationUser = applicationUser,
                    Token = userToken
                }
            );

            var data = await query.FirstOrDefaultAsync();
            if (data == null)
                return ReasonResult<AuthResponse>.BadRequest(new Reason[] { new Reason("RefreshTokenNotFound", "Cannot find Refresh Token") });

            var user = data.ApplicationUser;
            string refreshToken = data.Token.Value;

            bool verify = await _userManager.VerifyUserTokenAsync(user, AuthSettings.RefreshTokenProvider, AuthSettings.RefreshTokenName, refreshToken);

            if (!verify)
                return ReasonResult<AuthResponse>.BadRequest(new Reason[] { new Reason("RefreshTokenInvalid", "Refresh Token is invalid") });

            string accessToken = _jwtGenerator.GenerateToken(user, out var expires);
            var output = new AuthResponse(accessToken, refreshToken, expires);

            return ReasonResult<AuthResponse>.Success(output);
        }
    }
}
