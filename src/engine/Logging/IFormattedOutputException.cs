using Microsoft.Extensions.Logging;

namespace Db4Wd.Logging;

public interface IFormattedOutputException
{
    void Log(ILogger logger);
}