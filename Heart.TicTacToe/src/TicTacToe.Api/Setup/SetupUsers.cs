using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Api.Logic.Users;

namespace TicTacToe.Api.Setup
{
    public static partial class SetupUtils
    {
        public static IServiceCollection ConfigureUsers(
            this IServiceCollection services)
        {
            services.AddTransient<IUserManager, UserManager>();

            return services;
        }
    }
}
