namespace DbForward.Engine.Features.Environment;

internal sealed class Configuration : IFeatureConfiguration
{
    /// <inheritdoc />
    public void Configure(FeatureContext context) => context.AddRoute("env");
}