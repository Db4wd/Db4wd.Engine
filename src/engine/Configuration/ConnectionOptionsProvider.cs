using DbForward.Features;
using Microsoft.Extensions.Configuration;

namespace DbForward.Configuration;

internal sealed class ConnectionOptionsProvider(ConnectionOptions options) : ConfigurationProvider
{
    /// <inheritdoc />
    public override void Load()
    {
        TryAddProperty(nameof(ConnectionOptions.ConnectionString), options.ConnectionString);
        TryAddProperty(nameof(ConnectionOptions.Host), options.Host);
        TryAddPort(options.Port);
        TryAddProperty(nameof(ConnectionOptions.Database), options.Database);
        TryAddProperty(nameof(ConnectionOptions.UserId), options.UserId);
        TryAddProperty(nameof(ConnectionOptions.Password), options.Password);

        foreach (var (key, value) in options.Properties)
        {
            Data[key] = value;
        }
    }

    private void TryAddProperty(string property, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        Data[property] = value;
    }

    private void TryAddPort(uint? value)
    {
        if (value == null)
            return;

        Data[nameof(ConnectionOptions.Port)] = value.ToString();
    }
}