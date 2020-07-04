using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TicTacToe.Db.Models;

namespace TicTacToe.Api.Logic.Auth
{
    public interface IJwtGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, out DateTimeOffset expires);
        SecurityKey GenerateValidationKey();
    }
}
