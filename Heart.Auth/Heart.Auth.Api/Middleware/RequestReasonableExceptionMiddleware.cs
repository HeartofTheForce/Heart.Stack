using System.IO;
using System.Threading.Tasks;
using Heart.Auth.Logic.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Heart.Auth.Logic.Middleware
{
    public class RequestReasonableExceptionMiddleware : IMiddleware
    {
        private ILogger<RequestReasonableExceptionMiddleware> Logger { get; }

        public RequestReasonableExceptionMiddleware(ILogger<RequestReasonableExceptionMiddleware> logger)
        {
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ReasonableException reasonableException)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Reasons = reasonableException.Reasons
                }));

                Logger.LogInformation("Request Failed Reasonably, {@details}", new
                {
                    StatusCode = context.Response.StatusCode,
                    Reasons = reasonableException.Reasons,
                });
            }
        }
    }
}