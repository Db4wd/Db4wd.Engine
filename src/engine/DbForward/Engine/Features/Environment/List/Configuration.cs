using Vertical.Cli.Configuration;

namespace DbForward.Engine.Features.Environment.List;

internal sealed class Configuration : IFeatureConfiguration
{
    /// <inheritdoc />
    public void Configure(FeatureContext context) => context.AddFeature<EmptyModel, Handler>("env vars");
}