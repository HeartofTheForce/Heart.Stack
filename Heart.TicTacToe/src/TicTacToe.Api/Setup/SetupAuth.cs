using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TicTacToe.Api.Logic.Auth;
using TicTacToe.Api.Settings;
using TicTacToe.Db;
using TicTacToe.Db.Models;

namespace TicTacToe.Api.Setup
{
    public static partial class SetupUtils
    {
        public static IServiceCollection ConfigureAuth(
            this IServiceCollection services,
            AuthSettings settings)
        {
            services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    // Password settings.
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;

                    // Lockout settings.
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings.
                    options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<TicTacToeDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<RefreshTokenProvider>(AuthSettings.TokenProvider)
            ;

            services.Configure<RefreshTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(30);
            });

            services
                .AddTransient<IJwtGenerator, JwtGenerator>()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var sp = services.BuildServiceProvider();
                    var authGenerator = sp.GetRequiredService<IJwtGenerator>();

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = settings.Issuer,
                        ValidAudience = settings.Audience,
                        IssuerSigningKey = authGenerator.GenerateValidationKey(),
                    };
                })
            ;

            return services;
        }
    }
}
