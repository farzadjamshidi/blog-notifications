using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Blog.Notifications.Setup;

// Makes the message's trace ID visible in every log line —
// MassTransit propagates System.Diagnostics.Activity context through
// message headers automatically on publish/consume; this just surfaces
// it. Named "TraceId", not "RequestId" (which
// RemoveExtraPropertiesEnricher strips) — RequestId doesn't propagate
// across services; TraceId, the W3C Trace Context id, does
// (learning-notes/notes/49-distributed-tracing-correlation-ids.md).
internal class TraceIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var traceId = Activity.Current?.TraceId.ToString();
        if (!string.IsNullOrEmpty(traceId))
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", traceId));
        }
    }
}
