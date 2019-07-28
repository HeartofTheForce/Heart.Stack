using System.Text;
using Heart.Auth.Logic.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Heart.Auth.Logic.Jwt.AuthGenerators
{
    public class SymmetricAuthGenerator : IAuthGenerator
    {
        private AuthSettings AuthSettings { get; set; }

        public SymmetricAuthGenerator(IOptions<AuthSettings> options)
        {
            AuthSettings = options.Value;
        }

        public SigningCredentials GenerateSigningCredentials()
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthSettings.PrivateKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            return signingCredentials;
        }

        public SecurityKey GenerateValidationKey()
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthSettings.PrivateKey));
            return signingKey;
        }
    }
}