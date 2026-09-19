namespace Blog.Notifications.Setup;

public enum DatabaseType
{
    None,
    SqlServer,
    MongoDB
}

public class LogSetupConfig
{
    public DatabaseType DatabaseType { get; set; }
    public string ConnectionString { get; set; }
    public string MongoDatabaseName { get; set; }
    public string TableName { get; set; } = "Log";
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;
    public bool EnabledConsoleOutput { get; set; }
    public int CappedMaxSizeMb { get; set; } = 256;
}
