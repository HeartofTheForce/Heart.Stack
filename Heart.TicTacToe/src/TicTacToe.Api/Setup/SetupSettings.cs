using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TicTacToe.Api.Setup
{
    public static partial class SetupUtils
    {
        public static TSettings ConfigureSettings<TSettings>(
            this IServiceCollection services,
            IConfigurationSection configurationSection)
        where TSettings : class
        {
            services.Configure<TSettings>(configurationSection);
            return configurationSection.Get<TSettings>();
        }
    }
}
