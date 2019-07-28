using Microsoft.AspNetCore.Identity;

namespace Heart.Auth.Logic.Jwt
{
    public interface IJwtGenerator
    {
        string GenerateToken(IdentityUser identityUser);
    }
}