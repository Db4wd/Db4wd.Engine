using System.Diagnostics;
using DbForward.Engine.Extensions;
using DbForward.Engine.Logging;
using DbForward.Engine.Options;
using DbForward.Engine.Utilities;
using Microsoft.Extensions.Logging;
using Vertical.Cli.Routing;

namespace DbForward.Engine.Features.Environment.NewFile;

public sealed class Feature(IDb4Extension extension, ILogger<Feature> logger) : IAsyncCallSite<NewEnvironmentOptions>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(NewEnvironmentOptions options, CancellationToken cancellationToken)
    {
        await WriteFileAsync(options, cancellationToken);

        logger.LogInformation("{ok} Wrote new environment file {path}", OkValue.Default, options.OutputFile);

        if (!options.Edit)
            return 0;

        if (EnvironmentVariables.EditorProgram is not { Value:  not null } editorProgram)
        {
            logger.LogError("A text editor has not been configured ($DB4_EDITOR not set).");
            return -1;
        }

        var arguments = string.Format(EnvironmentVariables.EditorArgumentsFormat?.Value ?? "{0}", options.OutputFile);
        logger.LogDebug("Launching editor: {program} {args}", editorProgram, arguments);
        
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = editorProgram.Value,
            Arguments = arguments,
            UseShellExecute = true,
            CreateNoWindow = true,
            RedirectStandardInput = false,
            RedirectStandardOutput = false
        });

        await process!.WaitForExitAsync(cancellationToken);
        
        return 0;
    }

    private async Task WriteFileAsync(NewEnvironmentOptions options, CancellationToken cancellationToken)
    {
        await using var textWriter = new StreamWriter(File.Open(options.OutputFile.FullName, FileMode.Create));
        await extension.WriteEnvironmentFileAsync(textWriter, options, cancellationToken);
    }
}