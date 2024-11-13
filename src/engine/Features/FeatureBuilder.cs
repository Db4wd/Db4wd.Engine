using Microsoft.Extensions.DependencyInjection;
using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features;

/// <summary>
/// Builder used to construct features.
/// </summary>
public sealed class FeatureBuilder(IServiceProvider serviceProvider, IFeatureHost host)
{
    /// <summary>
    /// Configures commands for the provided parent.
    /// </summary>
    /// <param name="command">Parent command</param>
    /// <typeparam name="TOptions">Options type</typeparam>
    /// <returns>A reference to this instance</returns>
    public FeatureBuilder Configure<TOptions>(CliCommand<TOptions> command) where TOptions : GlobalOptions
    {
        var configurations = serviceProvider.GetServices<IFeatureConfiguration<TOptions>>();

        foreach (var configuration in configurations)
        {
            configuration.Configure(command, this);
        }

        return this;
    }
    
    /// <summary>
    /// Adds a handler.
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns>A reference to this instance</returns>
    public FeatureBuilder AddHandler<TOptions>(CliCommand<TOptions> command) where TOptions : GlobalOptions
    {
        command.HandleAsync(async (options, cancellationToken) => await host.ExecuteFeatureAsync(
            options, cancellationToken));
        
        return this;
    }

    /// <summary>
    /// Builds the root command.
    /// </summary>
    /// <param name="host">Host that orchestrates feature execution.</param>
    /// <param name="rootCommand">Root command instance.</param>
    /// <returns>The root command instance.</returns>
    public static RootCommand<GlobalOptions> Build(IFeatureHost host, RootCommand<GlobalOptions> rootCommand)
    {
        var services = new ServiceCollection()
            .Scan(types => types.FromAssemblyOf<FeatureBuilder>()
                .AddClasses(classes => classes.AssignableTo(typeof(IFeatureConfiguration<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime())
            .BuildServiceProvider();

        var instance = new FeatureBuilder(services, host);
        instance.Configure(rootCommand);

        return rootCommand;
    }
}