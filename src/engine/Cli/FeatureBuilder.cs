using Db4Wd.Engine;
using Db4Wd.Features;
using Microsoft.Extensions.DependencyInjection;
using Vertical.Cli.Configuration;

namespace Db4Wd.Cli;

public sealed class FeatureBuilder(IServiceProvider serviceProvider, EngineHost engine)
{
    public void Configure<TOptions>(CliCommand<TOptions> parent) where TOptions : GlobalOptions
    {
        var configurations = serviceProvider.GetServices<IFeatureConfiguration<TOptions>>();

        foreach (var configuration in configurations)
        {
            configuration.Configure(parent, this);
        }
    }

    public FeatureBuilder AddHandler<TOptions>(CliCommand<TOptions> command) where TOptions : GlobalOptions
    {
        command.HandleAsync(engine.ExecuteFeatureAsync);
        return this;
    }
}