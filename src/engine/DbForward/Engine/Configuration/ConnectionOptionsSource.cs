using DbForward.Engine.Models;
using Microsoft.Extensions.Configuration;

namespace DbForward.Engine.Configuration;

internal sealed class ConnectionOptionsSource(IConnectionOptions options) : IConfigurationSource
{
    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new ConnectionOptionsProvider(options);
}