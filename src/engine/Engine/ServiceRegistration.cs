using DbForward.Configuration;
using DbForward.Features;
using DbForward.Logging;
using DbForward.Services;
using DbForward.Services.Auditing;
using DbForward.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DbForward.Engine;

internal static class ServiceRegistration
{
    public static IServiceCollection AddEngine(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IFileSystem, PhysicalFileSystem>()
            .AddSingleton<IMigrationOperator, MigrationOperator>()
            .AddSingleton<IAgentContext, DefaultAgentContext>()
            .Scan(types => types.FromAssembliesOf(typeof(ServiceRegistration))
                .AddClasses(classes => classes.AssignableTo(typeof(IFeature<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo<ISourceAuditStep>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime())
            .AddSingleton<ISourceAuditor, SourceAuditor>()
            .AddSingleton<ISourceFileManager, SourceFileManager>()
            .AddSingleton(new TimerProvider(() => new StopwatchTimer()));
    }
    
    public static IServiceCollection AddExtensionServices(this IServiceCollection serviceCollection,
        IServiceCollection extensionServices)
    {
        foreach (var descriptor in extensionServices)
        {
            serviceCollection.Add(descriptor);
        }

        return serviceCollection;
    }

    public static IServiceCollection AddConfiguration<TOptions>(
        this IServiceCollection serviceCollection, 
        IConfigurationBuilder configurationBuilder,
        TOptions options) where TOptions : GlobalOptions
    {
        if (options.EnvironmentFile != null)
        {
            configurationBuilder.AddJsonFile(options.EnvironmentFile.FullName);
        }

        if (options is ConnectionOptions connectionOptions)
        {
            configurationBuilder.Add(new ConnectionOptionsSource(connectionOptions));
        }

        foreach (var provider in configurationBuilder.Sources)
        {
            StartupLogger.Log(LogLevel.Debug, "Added configuration provider {provider}", provider.GetType().Name);
        }

        return serviceCollection.AddSingleton<IConfiguration>(configurationBuilder.Build());
    }
}