using DbForward.Engine.Utilities;
using Spectre.Console;
using Vertical.Cli.Configuration;
using Vertical.Cli.Routing;

namespace DbForward.Engine.Features.Environment.List;

internal sealed class Handler : IAsyncCallSite<EmptyModel>
{
    /// <inheritdoc />
    public Task<int> HandleAsync(EmptyModel model, CancellationToken cancellationToken)
    {
        var variables = EnvironmentVariables
            .GetDescriptors()
            .OrderBy(dsc => dsc.Name)
            .ToArray();

        var table = new Table();
        table.AddColumns("Name", "Value", "Description");
        table.Border(TableBorder.None);
        table.Columns[0].PadRight(3);
        table.Columns[1].PadRight(3);
        table.Columns[2].PadRight(3);
        
        foreach (var variable in variables)
        {
            var value = variable.Value != null
                ? $"[orange3]{variable.Value}[/]"
                : "[grey42](not set)[/]";
            
            table.AddRow(
                $"{variable.Name}",
                value,
                $"{variable.Description}");
        }
        
        AnsiConsole.Write(table);
        
        return Task.FromResult(0);
    }
}