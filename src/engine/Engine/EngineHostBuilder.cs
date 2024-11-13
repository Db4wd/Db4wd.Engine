using DbForward.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbForward.Engine;

/// <summary>
/// Constructs new instances of <see cref="IEngineHost"/>
/// </summary>
public sealed class EngineHostBuilder(string name, string description)
{
    private readonly IServiceCollection services = new ServiceCollection();
    private readonly IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

    /// <summary>
    /// Configures the application's configuration builder.
    /// </summary>
    /// <param name="configure">A delegate that acts upon the given <see cref="IConfigurationBuilder"/></param>
    /// <returns>A reference to this instance</returns>
    public EngineHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configure)
    {
        configure(configurationBuilder);
        return this;
    }

    /// <summary>
    /// Configures extension services.
    /// </summary>
    /// <param name="configure">A delegate that acts upon the given <see cref="IServiceCollection"/></param>
    /// <returns>A reference to this instance</returns>
    public EngineHostBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(services);
        return this;
    }

    /// <summary>
    /// Adds an environment file option.
    /// </summary>
    /// <param name="variableKey">The name of the enviornment variable that references the file.</param>
    /// <returns>A reference to this instance.</returns>
    public EngineHostBuilder AddEnvironmentFileOption(string variableKey)
    {
        var value = Environment.GetEnvironmentVariable(variableKey);
        if (string.IsNullOrWhiteSpace(value))
            return this;

        configurationBuilder.AddJsonFile(value);
        return this;
    }

    /// <summary>
    /// Registers the extension.
    /// </summary>
    /// <typeparam name="T">Extension type</typeparam>
    /// <returns>A reference to this instance</returns>
    public EngineHostBuilder AddExtension<T>() where T : class, IDatabaseExtension
    {
        services.AddSingleton<IDatabaseExtension, T>();
        return this;
    }

    /// <summary>
    /// Builds the engine instance.
    /// </summary>
    /// <returns><see cref="IEngineHost"/></returns>
    public IEngineHost Build() => new EngineHost(
        configurationBuilder,
        services,
        name,
        description);
}