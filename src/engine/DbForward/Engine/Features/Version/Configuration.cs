using Vertical.Cli.Configuration;

namespace DbForward.Engine.Features.Version;

internal sealed class Configuration : IFeatureConfiguration
{
    /// <inheritdoc />
    public void Configure(FeatureContext context) => context.AddFeature<EmptyModel, Feature>("version");
}