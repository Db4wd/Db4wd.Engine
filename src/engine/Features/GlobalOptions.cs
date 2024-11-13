using Microsoft.Extensions.Logging;
using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward;

/// <summary>
/// Base class for options
/// </summary>
public class GlobalOptions
{
    internal const string OptionGroup = "Global options";
    
    public LogLevel LogLevel { get; set; }
    
    public bool DebugMode { get; set; }
    
    public FileInfo? EnvironmentFile { get; set; }
    
    public bool ShortSha { get; set; }
}