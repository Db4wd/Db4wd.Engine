using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DbForward.Utilities;

public static class LiveEditing
{
    public static void TryEditFile(string path, ILogger logger)
    {
        var editor = Environment.GetEnvironmentVariable("PGFWD_EDITOR");

        if (editor == null)
        {
            logger.LogInformation("An editor has not been configured (set $PGFWD_EDITOR).");
            return;
        }

        var process = Process.Start(new ProcessStartInfo(editor, path));

        if (process == null)
        {
            logger.LogError("Failed to start process {process}", editor);
            return;
        }

        while (!process.HasExited)
        {
            Thread.Sleep(1000);
        }
    }
}