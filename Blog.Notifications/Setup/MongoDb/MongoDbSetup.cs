using MongoDB.Driver;
using Serilog;

namespace Blog.Notifications.Setup.MongoDb;
internal static class MongoDbSetup
{
    public static LoggerConfiguration AddMongoDb(this LoggerConfiguration setup, LogSetupConfig config)
    {
        var mongoClient = new MongoClient(config.ConnectionString);
        var database = mongoClient.GetDatabase(config.MongoDatabaseName);
        return setup.WriteTo.MongoDBCapped(
            database,
            restrictedToMinimumLevel: config.LogLevel.ToSerilogLogLevel(),
            cappedMaxSizeMb: config.CappedMaxSizeMb,
            collectionName: config.TableName,
            mongoDBJsonFormatter: new MongoJsonFormatter());
    }

}
