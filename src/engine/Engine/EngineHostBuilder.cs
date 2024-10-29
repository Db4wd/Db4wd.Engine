using Db4Wd.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Db4Wd.Engine;

public sealed class EngineHostBuilder(string rootContext, string description)
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly IConfigurationBuilder _configurationBuilder = new ConfigurationBuilder();

    /// <summary>
    /// Configures additional services the extension requires.
    /// </summary>
    /// <param name="configure">Delegate used to manage a service collection</param>
    /// <returns>A reference to this instance</returns>
    public EngineHostBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(_services);
        return this;
    }

    /// <summary>
    /// Configures a <see cref="IConfigurationManager"/>
    /// </summary>
    /// <param name="configure">Delegate used to perform additional configuration</param>
    /// <returns>A reference to this instance</returns>
    public EngineHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configure)
    {
        configure(_configurationBuilder);
        return this;
    }

    /// <summary>
    /// Registers the extension in the service collection.
    /// </summary>
    /// <typeparam name="T">Extension implementation type</typeparam>
    /// <returns>A reference to this instance</returns>
    public EngineHostBuilder AddExtension<T>() where T : class, IExtension => ConfigureServices(services =>
        services.AddSingleton<IExtension, T>());

    public IEngineHost Build() => new EngineHost(
        _services,
        _configurationBuilder,
        rootContext,
        description);
}