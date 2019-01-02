using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog;

namespace Katalye.Host.Middleware
{
    public class AspNetCoreLoggingFilter : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationId = Guid.NewGuid().ToString("N");

            MappedDiagnosticsLogicalContext.Set("CorrelationId", correlationId);
            MappedDiagnosticsLogicalContext.Set("CorrelationBlob", correlationId);

            context.Response.OnStarting(() =>
            {
                var localCorrelationId = MappedDiagnosticsLogicalContext.Get("CorrelationId");
                context.Response.Headers.Add("X-Katalye-Correlation-Id", localCorrelationId);

                var localCorrelationBlob = MappedDiagnosticsLogicalContext.Get("CorrelationBlob");
                context.Response.Headers.Add("X-Katalye-Correlation-Blob", localCorrelationBlob);

                return Task.CompletedTask;
            });

            await next(context);
        }
    }
}