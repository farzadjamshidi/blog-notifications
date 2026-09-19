using Serilog.Core;
using Serilog.Events;

namespace Blog.Notifications.Setup;

internal class RemoveExtraPropertiesEnricher : ILogEventEnricher
{
    private static readonly string[] _propertiesToRemove = new[]
    {
        "ActionId",
        "ActionName",
        "RequestId",
        "ConnectionId"
    };

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        foreach (var property in _propertiesToRemove)
        {
            logEvent.RemovePropertyIfPresent(property);
        }
    }
}
