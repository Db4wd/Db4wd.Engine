using DbForward.Engine.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbForward.Engine;

public sealed class DbForwardEngine
{
    private readonly string app;
    private readonly IEnumerable<Action<IConfigurationBuilder>> configurationActions;
    private readonly IEnumerable<Action<IServiceCollection>> serviceCollectionActions;

    internal DbForwardEngine(string app,
        IEnumerable<Action<IConfigurationBuilder>> configurationActions,
        IEnumerable<Action<IServiceCollection>> serviceCollectionActions)
    {
        this.app = app;
        this.configurationActions = configurationActions;
        this.serviceCollectionActions = serviceCollectionActions;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default)
    {
        return await FeatureContext.RunAsync(app,
            configurationActions,
            serviceCollectionActions,
            args,
            cancellationToken);
    }
}