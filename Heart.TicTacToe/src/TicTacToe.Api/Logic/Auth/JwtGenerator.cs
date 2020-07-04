using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TicTacToe.Api.Settings;
using TicTacToe.Db.Models;

namespace TicTacToe.Api.Logic.Auth
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly AuthSettings _authSettings;

        public JwtGenerator(
            IOptions<AuthSettings> options)
        {
            _authSettings = options.Value;
        }

        public string GenerateToken(ApplicationUser applicationUser, out DateTimeOffset expires)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.PrivateKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = {
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Id.ToString()),
            };

            expires = DateTime.UtcNow.AddMinutes(_authSettings.ExpireInMinutes);

            var token = new JwtSecurityToken(_authSettings.Issuer, _authSettings.Audience, claims, null, expires.UtcDateTime, signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public SecurityKey GenerateValidationKey()
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.PrivateKey));
            return signingKey;
        }
    }
}
