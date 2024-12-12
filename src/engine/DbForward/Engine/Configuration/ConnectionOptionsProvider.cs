using DbForward.Engine.Models;
using Microsoft.Extensions.Configuration;

namespace DbForward.Engine.Configuration;

internal sealed class ConnectionOptionsProvider(IConnectionOptions options) : ConfigurationProvider
{
    /// <inheritdoc />
    public override void Load()
    {
        TryAddProperty("connectionString", options.ConnectionString);
        TryAddProperty("host", options.Host);
        TryAddProperty("port", options.Port?.ToString());
        TryAddProperty("database", options.Database);
        TryAddProperty("userid", options.UserId);
        TryAddProperty("password", options.Password);

        foreach (var (key, value) in options.Properties)
        {
            TryAddProperty(key, value);    
        }
        
        base.Load();
    }

    private void TryAddProperty(string propertyName, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        Data[propertyName] = value;
    }
}