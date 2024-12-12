using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbForward.Engine;

public sealed class DbForwardEngineBuilder(string applicationName)
{
    private readonly IServiceCollection services = new ServiceCollection();
    private readonly List<Action<IConfigurationBuilder>> configurationActions = [];
    private readonly List<Action<IServiceCollection>> serviceCollectionActions = [];

    public DbForwardEngineBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configure)
    {
        configurationActions.Add(configure);
        return this;
    }
    
    public DbForwardEngineBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        serviceCollectionActions.Add(configure);
        return this;
    }

    public DbForwardEngine Build() => new(applicationName, configurationActions, serviceCollectionActions);
}