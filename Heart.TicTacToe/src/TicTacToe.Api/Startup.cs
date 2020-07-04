using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Api.Settings;
using TicTacToe.Api.Setup;
using TicTacToe.Db;

namespace TicTacToe.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var ticTacToeSettings = services.ConfigureSettings<TicTacToeSettings>(_configuration.GetSection("TicTacToe"));
            var authSettings = services.ConfigureSettings<AuthSettings>(_configuration.GetSection("Auth"));

            services.AddControllers();
            services.ConfigureDb(ticTacToeSettings);
            services.ConfigureAuth(authSettings);
            services.ConfigureUsers();

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "TicTacToe Api";
                };
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IServiceProvider serviceProvider)
        {
            serviceProvider.GetRequiredService<TicTacToeDbContext>().Database.Migrate();

            app.UseHttpsRedirection();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
