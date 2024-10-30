using Microsoft.Extensions.Logging;

namespace Db4Wd.Services;

public enum MigrationOperatorMode
{
    Apply,
    Rollback
}

public interface IMigrationOperatorOptions
{
    DirectoryInfo BasePath { get; }
    
    string? MatchPattern { get; }
    
    MigrationOperatorMode OperatorMode { get; }
    
    Guid? TargetMigrationId { get; }
    
    int? TargetVersion { get; }
    
    LogLevel StatementLogLevel { get; }
}