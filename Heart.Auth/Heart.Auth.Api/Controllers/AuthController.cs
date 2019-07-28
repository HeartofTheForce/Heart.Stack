using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heart.Auth.Logic.Exceptions;
using Heart.Auth.Logic.Jwt;
using Heart.Auth.Logic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Heart.Auth.Api.Controllers
{
    public class AuthController : Controller
    {
        public UserManager<IdentityUser> UserManager { get; }
        public SignInManager<IdentityUser> SignInManager { get; }
        public IJwtGenerator JwtGenerator { get; }

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJwtGenerator jwtGenerator)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            JwtGenerator = jwtGenerator;
        }

        [HttpPost("api/auth/register")]
        public async Task<IActionResult> Register([FromBody] AuthRequestModel authRequestModel)
        {
            var newUser = new IdentityUser()
            {
                UserName = authRequestModel.Email,
                Email = authRequestModel.Email,
            };

            var identityResult = await UserManager.CreateAsync(newUser, authRequestModel.Password);

            var reasons = new List<ReasonableException.Reason>();
            if (identityResult.Succeeded)
            {
                return Ok();
            }
            else
            {
                throw ReasonableExceptionFromIdentityResult(identityResult);
            }
        }

        [HttpPost("api/auth/login")]
        public async Task<ActionResult<AuthResponseModel>> Login([FromBody] AuthRequestModel authRequestModel)
        {
            var user = await UserManager.FindByEmailAsync(authRequestModel.Email);
            if (user != null)
            {
                var signInResult = await SignInManager.CheckPasswordSignInAsync(user, authRequestModel.Password, lockoutOnFailure: true);

                if (signInResult.Succeeded)
                {
                    return Ok(new AuthResponseModel(JwtGenerator.GenerateToken(user)));
                }
                else
                {
                    throw ReasonableExceptionFromSignInResult(signInResult);
                }
            }
            else
            {
                throw new ReasonableException(new ReasonableException.Reason("UserNotFound", "Cannot find User."));
            }
        }


        private ReasonableException ReasonableExceptionFromSignInResult(Microsoft.AspNetCore.Identity.SignInResult signInResult)
        {
            var output = new List<ReasonableException.Reason>();

            if (signInResult.IsLockedOut)
            {
                output.Add(new ReasonableException.Reason("LockedOut", "User is locked out."));
            }
            if (signInResult.IsNotAllowed)
            {
                output.Add(new ReasonableException.Reason("IsNotAllowed", "User is not allowed to sign in."));
            }

            if (!signInResult.IsLockedOut && !signInResult.IsNotAllowed && !signInResult.RequiresTwoFactor)
            {
                output.Add(new ReasonableException.Reason("InvalidPassword", "Password does not match."));
            }

            return new ReasonableException(output);
        }

        private ReasonableException ReasonableExceptionFromIdentityResult(IdentityResult identityResult)
        {
            return new ReasonableException(identityResult.Errors.Select(x => new ReasonableException.Reason(x.Code, x.Description)));
        }
    }
}