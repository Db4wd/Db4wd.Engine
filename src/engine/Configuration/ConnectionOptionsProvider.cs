using Db4Wd.Extensions;
using Microsoft.Extensions.Configuration;

namespace Db4Wd.Configuration;

public sealed class ConnectionOptionsProvider(IConnectionOptions options) : ConfigurationProvider
{
    /// <inheritdoc />
    public override void Load()
    {
        Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        TryAddValue(nameof(options.ConnectionString), options.ConnectionString);
        TryAddValue(nameof(options.Host), options.Host);
        TryAddValue(nameof(options.Database), options.Database);
        TryAddValue(nameof(options.UserId), options.UserId);
        TryAddValue(nameof(options.Password), options.Password);
        TryAddValue(nameof(options.Port), options.Port?.ToString());

        foreach (var (key, value) in options.Properties)
        {
            TryAddValue(key, value);
        }
    }

    private void TryAddValue(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        Data[key] = value.ToString();
    }
}