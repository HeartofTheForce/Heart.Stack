using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

namespace TicTacToe.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ConfigureLogging();

            try
            {
                Log.Information("Starting web host");

                CreateHostBuilder(args)
                    .Build()
                    .Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();


        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(new CompactJsonFormatter(new JsonValueFormatter(null)))
                .CreateLogger();
        }
    }
}
