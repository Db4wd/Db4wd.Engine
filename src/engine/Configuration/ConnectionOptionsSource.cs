using DbForward.Features;
using Microsoft.Extensions.Configuration;

namespace DbForward.Configuration;

internal sealed class ConnectionOptionsSource(ConnectionOptions options) : IConfigurationSource
{
    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ConnectionOptionsProvider(options);
    }
}