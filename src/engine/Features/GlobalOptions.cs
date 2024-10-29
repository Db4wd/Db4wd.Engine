using Microsoft.Extensions.Logging;

namespace Db4Wd.Features;

public class GlobalOptions
{
    public LogLevel LogLevel { get; set; }
    
    public bool Debug { get; set; }
    
    public TimeSpan TimeZoneOffset { get; set; }
    
    public FileInfo? EnvironmentFile { get; set; }
}