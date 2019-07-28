using System;
using System.IO;
using System.Threading.Tasks;
using Heart.Auth.Logic.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Heart.Auth.Logic.Middleware
{
    public class RequestExceptionHandlingMiddleware : IMiddleware
    {
        private ReasonableException.Reason InternalServerError => new ReasonableException.Reason("InternalServerError", "An unhandled exception occured");

        private ILogger<RequestExceptionHandlingMiddleware> Logger { get; }

        public RequestExceptionHandlingMiddleware(ILogger<RequestExceptionHandlingMiddleware> logger)
        {
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (errorFeature != null)
            {
                await HandleException(context, errorFeature.Error);
            }
            else
            {
                try
                {
                    await next(context);
                }
                catch (Exception ex)
                {
                    await HandleException(context, ex);
                }
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var reasons = new ReasonableException.Reason[] { InternalServerError };
            await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                Reasons = reasons
            }));

            Logger.LogInformation("Request Failed, {@details}", new
            {
                StatusCode = context.Response.StatusCode,
                Reasons = reasons,
            });
        }
    }
}