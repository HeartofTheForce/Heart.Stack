using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Heart.Auth.Logic.Middleware
{
    public class RequestLoggingMiddleware : IMiddleware
    {
        private ILogger<RequestLoggingMiddleware> Logger { get; }

        public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
        {
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Logger.LogInformation("Request Started, {@details}", new
            {
                Path = context.Request.Path.Value,
                Method = context.Request.Method,
                QueryString = context.Request.QueryString.Value,
            });

            Stopwatch sw = Stopwatch.StartNew();

            bool succeed = false;
            try
            {
                await next(context);
                succeed = true;
            }
            finally
            {
                sw.Stop();
                Logger.LogInformation("Request Finished, {@details}", new
                {
                    Succeed = succeed,
                    ElapsedMilliseconds = sw.ElapsedMilliseconds,
                });
            }
        }
    }
}