using Heart.Auth.Logic.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heart.Auth.Logic.Setup
{
    public static partial class Setup
    {
        public static IServiceCollection ConfigureSettings(
            this IServiceCollection services,
            IConfiguration configuration,
            out AuthSettings authSettings)
        {
            authSettings = services.ConfigureAndGet<AuthSettings>(configuration.GetSection("AuthSettings"));

            return services;
        }

        private static TOptions ConfigureAndGet<TOptions>(
            this IServiceCollection services,
            IConfigurationSection configurationSection)
        where TOptions : class
        {
            services.Configure<TOptions>(configurationSection);
            return configurationSection.Get<TOptions>();
        }
    }
}