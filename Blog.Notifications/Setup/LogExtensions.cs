using Serilog.Events;

namespace Blog.Notifications.Setup;

public static class LogExtensions
{
    public static LogEventLevel ToSerilogLogLevel(this LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            _ => throw new NotImplementedException($"Unknown logLevel {logLevel}")
        };
    }
}
