using Microsoft.IdentityModel.Tokens;

namespace Heart.Auth.Logic.Jwt.AuthGenerators
{
    public interface IAuthGenerator
    {
        SigningCredentials GenerateSigningCredentials();
        SecurityKey GenerateValidationKey();
    }
}