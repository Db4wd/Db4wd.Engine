using DbForward.Engine.Options;

namespace DbForward.Engine.Features.Environment.NewFile;

internal sealed class Configuration : IFeatureConfiguration
{
    /// <inheritdoc />
    public void Configure(FeatureContext context) => context.AddFeature<NewEnvironmentOptions, Feature>("env new-file");
}