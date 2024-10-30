using Db4Wd.Logging;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Parsing;

public sealed class MigrationSourceException(string message, string context, int? lineNumber = null) 
    : Exception(message), IFormattedOutputException
{
    /// <inheritdoc />
    public void Log(ILogger logger)
    {
        if (lineNumber.HasValue)
        {
            logger.LogError("Error in source file {context} at line {line}: {message}",
                context,
                lineNumber,
                message);
            return;
        }
        
        logger.LogError("Error in source file {context}: {message}",
            context,
            message);
    }
}