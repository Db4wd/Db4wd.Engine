using Db4Wd.Extensions;
using Microsoft.Extensions.Configuration;

namespace Db4Wd.Configuration;

public sealed class ConnectionOptionsSource(IConnectionOptions options) : IConfigurationSource
{
    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new ConnectionOptionsProvider(options);
}