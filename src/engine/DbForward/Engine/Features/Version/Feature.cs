using DbForward.Engine.Extensions;
using DbForward.Engine.Logging;
using Microsoft.Extensions.Logging;
using Vertical.Cli.Configuration;
using Vertical.Cli.Routing;

namespace DbForward.Engine.Features.Version;

internal sealed class Feature(IDb4Extension extension, ILogger<Feature> logger) : IAsyncCallSite<EmptyModel>
{
    /// <inheritdoc />
    public Task<int> HandleAsync(EmptyModel _, CancellationToken cancellationToken)
    {
        logger.LogInformation("{c}", new InfoValue("Copyright (C) 2024 db-forward contributors"));
        logger.LogInformation("db4 engine: {v}", "1.0.0");
        logger.LogInformation("provider: {provider}={version}", extension.Id, extension.Version);
        
        return Task.FromResult(0);
    }
}