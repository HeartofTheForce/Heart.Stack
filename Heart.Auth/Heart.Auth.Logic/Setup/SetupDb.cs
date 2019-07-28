using Heart.Auth.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Heart.Auth.Logic.Setup
{
    public static partial class Setup
    {
        public static IServiceCollection ConfigureDb(this IServiceCollection services)
        {
            return services
                .AddDbContext<AuthDbContext>(o => o.UseInMemoryDatabase("Auth.db"))
            ;
        }
    }
}