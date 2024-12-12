using DbForward.Engine.Extensions;
using DbForward.Engine.Models;
using DbForward.Engine.Utilities;

namespace DbForward.Postgres;

public class PostgresDb4Extension : IDb4Extension
{
    /// <inheritdoc />
    public string Id => "postgres";

    /// <inheritdoc />
    public string Version => "1.1.0";

    /// <inheritdoc />
    public async Task WriteEnvironmentFileAsync(TextWriter textWriter,
        INewEnvironmentOptions options,
        CancellationToken cancellationToken)
    {
        var properties = new Dictionary<string, string?>
        {
            ["host"] = GetPropertyValue(options, options.Host, "localhost"),
            ["port"] = GetPropertyValue(options, options.Port?.ToString(), "5432"),
            ["database"] = GetPropertyValue(options, options.Database, "<database>"),
            ["userId"] = GetPropertyValue(options, options.UserId, "root"),
            ["password"] = GetPropertyValue(options, options.Password, "<password>")
        };

        if (options.ConnectionString != null)
        {
            properties["connectionString"] = options.ConnectionString;
        }

        foreach (var (key, value) in options.Properties)
        {
            properties[key] = value;
        }

        await textWriter.WriteJsonAsync(properties);
        await textWriter.WriteLineAsync();
    }

    private static string? GetPropertyValue(INewEnvironmentOptions options, string? value, string defaultValue)
    {
        return options.ConnectionString == null
            ? value ?? defaultValue
            : null;
    }
}