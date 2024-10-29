using Microsoft.Extensions.DependencyInjection;

namespace Db4Wd.Startup;

internal static class StartupExtensions
{
    internal static IServiceCollection AddStartupServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .Scan(types => types.FromAssemblies(typeof(StartupExtensions).Assembly)
                .AddClasses(classes => classes.AssignableTo<IStartupAction>())
                .AsImplementedInterfaces()
                .WithTransientLifetime())
            .AddSingleton<StartupActionAggregate>();
    }
}