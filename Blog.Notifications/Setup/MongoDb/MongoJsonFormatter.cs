using MongoDB.Bson;
using Serilog.Sinks.MongoDB;

namespace Blog.Notifications.Setup.MongoDb;

// Copied and edited from https://github.com/ChangemakerStudios/serilog-sinks-mongodb/blob/dev/src/Serilog.Sinks.MongoDB/Sinks/MongoDB/MongoDBJsonFormatter.cs
internal class MongoJsonFormatter : DefaultJsonFormatter
{
    private static readonly IDictionary<Type, Action<object, TextWriter>> _dateTimeWriters;

    static MongoJsonFormatter()
    {
        _dateTimeWriters = new Dictionary<Type, Action<object, TextWriter>>
        {
            { typeof(DateTime), (v, w) => WriteDateTime((DateTime)v, w) },
            { typeof(DateTimeOffset), (v, w) => WriteOffset((DateTimeOffset)v, w) }
        };
    }

    public MongoJsonFormatter(
        bool omitEnclosingObject = false,
        string closingDelimiter = null,
        bool renderMessage = true,
        IFormatProvider formatProvider = null,
        bool renderMessageTemplate = false)
        : base(omitEnclosingObject, closingDelimiter, renderMessage, formatProvider, renderMessageTemplate) { }

    protected override void WriteJsonProperty(string name, object value, ref string precedingDelimiter, TextWriter output)
    {
        name = name.Replace('$', '_').Replace('.', '-');

        if (value != null && _dateTimeWriters.TryGetValue(value.GetType(), out var action))
        {
            output.Write(precedingDelimiter);
            output.Write("\"");
            output.Write(name);
            output.Write("\":");
            action(value, output);
            precedingDelimiter = ",";
        }
        else
        {
            base.WriteJsonProperty(name, value, ref precedingDelimiter, output);
        }
    }

    private static void WriteOffset(DateTimeOffset value, TextWriter output)
    {
        output.Write($"{{ \"$date\" : {BsonUtils.ToMillisecondsSinceEpoch(value.UtcDateTime)} }}");
    }

    private static void WriteDateTime(DateTime value, TextWriter output)
    {
        output.Write($"{{ \"$date\" : {BsonUtils.ToMillisecondsSinceEpoch(value)} }}");
    }
}
