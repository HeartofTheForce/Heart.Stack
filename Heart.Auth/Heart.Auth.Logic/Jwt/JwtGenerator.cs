using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Heart.Auth.Logic.Jwt.AuthGenerators;
using Heart.Auth.Logic.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Heart.Auth.Logic.Jwt
{
    public class JwtGenerator : IJwtGenerator
    {
        public AuthSettings AuthSettings { get; }
        public IAuthGenerator AuthGenerator { get; }

        public JwtGenerator(
            IOptions<AuthSettings> options,
            IAuthGenerator authGenerator)
        {
            AuthSettings = options.Value;
            AuthGenerator = authGenerator;
        }

        public string GenerateToken(IdentityUser identityUser)
        {
            var signingCredentials = AuthGenerator.GenerateSigningCredentials();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, identityUser.UserName),
                new Claim(ClaimTypes.Email, identityUser.Email),
            };

            var expires = DateTime.UtcNow.AddMinutes(AuthSettings.ExpireInMinutes);

            var token = new JwtSecurityToken(AuthSettings.Issuer, AuthSettings.Audience, claims, null, expires, signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}