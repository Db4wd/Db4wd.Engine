using Microsoft.Extensions.DependencyInjection;

namespace Db4Wd.Features;

public static class FeatureExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .Scan(types => types.FromAssemblies(typeof(FeatureExtensions).Assembly)
                .AddClasses(static classes => classes.AssignableTo(typeof(IFeature<>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

    }
}