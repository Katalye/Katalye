using System;
using Hangfire.Client;
using Hangfire.Server;
using NLog;

namespace Katalye.Host.Middleware
{
    public class HangfireLoggingFilter : IServerFilter, IClientFilter
    {
        public void OnPerforming(PerformingContext filterContext)
        {
            var correlationId = Guid.NewGuid().ToString("N");
            MappedDiagnosticsLogicalContext.Set("CorrelationId", correlationId);

            var correlationBlob = filterContext.GetJobParameter<string>("CorrelationBlob");
            MappedDiagnosticsLogicalContext.Set("CorrelationBlob", $"{correlationBlob}:{correlationId}");
        }

        public void OnPerformed(PerformedContext filterContext)
        {
        }

        public void OnCreating(CreatingContext filterContext)
        {
            var correlationId = MappedDiagnosticsLogicalContext.Get("CorrelationId");
            filterContext.SetJobParameter("CorrelationId", correlationId);

            var correlationBlob = MappedDiagnosticsLogicalContext.Get("CorrelationBlob");
            filterContext.SetJobParameter("CorrelationBlob", correlationBlob);
        }

        public void OnCreated(CreatedContext filterContext)
        {
        }
    }
}