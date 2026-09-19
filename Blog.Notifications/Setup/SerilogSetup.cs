using Blog.Notifications.Setup.MongoDb;
using Serilog;
using Serilog.Events;

namespace Blog.Notifications.Setup;

internal static class SerilogSetup
{
    public static void AddSerilog(LogSetupConfig config)
    {
        var setup = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.With<RemoveExtraPropertiesEnricher>()
            .Enrich.With<TraceIdEnricher>()
            .Enrich.WithProperty("Service", "blog-notifications")
            .MinimumLevel.Is(config.LogLevel.ToSerilogLogLevel())
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error);

        if (config.EnabledConsoleOutput)
        {
            setup = setup.AddConsoleLogging(config);
        }

        setup = config.DatabaseType switch
        {
            DatabaseType.None => setup,
            DatabaseType.MongoDB => setup.AddMongoDb(config),
            _ => throw new NotImplementedException(),
        };

        Log.Logger = setup.CreateLogger();
    }

    private static LoggerConfiguration AddConsoleLogging(this LoggerConfiguration setup, LogSetupConfig config)
    {
        Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
        return setup.WriteTo.Console(config.LogLevel.ToSerilogLogLevel());
    }
}
