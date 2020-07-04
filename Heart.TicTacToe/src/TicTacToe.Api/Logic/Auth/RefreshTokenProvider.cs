using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TicTacToe.Db.Models;

namespace TicTacToe.Api.Logic.Auth
{
    public class RefreshTokenProvider : DataProtectorTokenProvider<ApplicationUser>
    {
        public RefreshTokenProvider(
            IDataProtectionProvider dataProtectionProvider,
            IOptions<RefreshTokenProviderOptions> options,
            ILogger<RefreshTokenProvider> logger)
        : base(dataProtectionProvider, options, logger)
        {
        }
    }

    public class RefreshTokenProviderOptions : DataProtectionTokenProviderOptions { }

}
