namespace DbForward.Engine.Features;

internal interface IFeatureConfiguration
{
    void Configure(FeatureContext context);
}