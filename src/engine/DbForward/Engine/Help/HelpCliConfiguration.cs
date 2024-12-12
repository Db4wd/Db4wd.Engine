using DbForward.Engine.Features;
using DbForward.Engine.Options;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Routing;

namespace DbForward.Engine.Help;

internal sealed class HelpCliConfiguration : IFeatureConfiguration
{
    private record GroupedParameter(CliParameter Parameter)
    {
        private const string ConnectionOptionsGroup = "Connection options:";
        private const string GlobalOptionsGroup = "Global options:";
        private const string OptionsGroup = "Options:";
        
        public sealed class GroupComparer : IComparer<IGrouping<string, CliParameter>>
        {
            /// <inheritdoc />
            public int Compare(IGrouping<string, CliParameter>? x, IGrouping<string, CliParameter>? y)
            {
                return Comparer<char?>.Default.Compare(SortKeyOf(x), SortKeyOf(y));
            }

            private static char? SortKeyOf(IGrouping<string, CliParameter>? grouping) => grouping?.Key switch
            {
                ConnectionOptionsGroup => 'b',
                GlobalOptionsGroup => 'c',
                not null => 'a',
                _ => null
            };
        }
        
        public string Section => Parameter.ModelType.Name switch
        {
            nameof(ConnectionOptions) => ConnectionOptionsGroup,
            nameof(GlobalOptions) => GlobalOptionsGroup,
            _ => OptionsGroup
        };
    }
    
    /// <inheritdoc />
    public void Configure(FeatureContext context)
    {
        var resources = HelpResources.ResourceManager;
        var app = context.CliBuilder.ApplicationName;
        var formatterOptions = new HelpFormattingOptions
        {
            RouteDescriptionFormatter = route => resources.GetString(ResolveHelpTopic(route, app)) ?? string.Empty,
            ParameterDescriptionFormatter = param => resources.GetString(ResolveHelpTopic(param, app)) ?? string.Empty,
            OptionsGroupsProvider = parameters => parameters
                .Order(CliParameterIdSortComparer.Instance)
                .Select(param => new GroupedParameter(param))
                .GroupBy(param => param.Section, param => param.Parameter)
                .Order(new GroupedParameter.GroupComparer())
        };

        context.CliBuilder.MapHelpSwitch(() => HelpProviders.CreateCompactFormatProvider(formatterOptions));
    }

    private static string ResolveHelpTopic(object target, string app)
    {
        switch (target)
        {
            case RouteDefinition route:
                var path = route.Path.Pattern;
                return path == app ? "root" : $"root {path[(path.IndexOf(' ') + 1)..]}";
            
            case CliParameter parameter:
                return $"{parameter.ModelType.Name}.{parameter.BindingName}".ToLower();
            
            default:
                throw new InvalidOperationException();
        }
    }
}