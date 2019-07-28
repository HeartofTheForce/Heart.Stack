using Heart.Auth.Logic.Middleware;
using Heart.Auth.Logic.Settings;
using Heart.Auth.Logic.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heart.Auth.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
            ;

            services
                .ConfigureSettings(Configuration, out AuthSettings authSettings)
                .ConfigureDb()
                .ConfigureAuth(authSettings)
            ;

            services
                .AddScoped<RequestLoggingMiddleware>()
                .AddScoped<RequestExceptionHandlingMiddleware>()
                .AddScoped<RequestReasonableExceptionMiddleware>()
            ;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app
                .UseExceptionHandler(errorApp =>
                {
                    errorApp
                        .UseMiddleware<RequestExceptionHandlingMiddleware>()
                    ;
                })
                .UseMiddleware<RequestLoggingMiddleware>()
                .UseMiddleware<RequestReasonableExceptionMiddleware>()
                .UseAuthentication()
                .UseMvc()
            ;
        }
    }
}
