using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using Microsoft.Extensions.Logging;

namespace DbForward.Services;

public record MigrationParameters(
    SourceOperation Operation,
    IAgentContext AgentContext,
    IFileSystem FileSystem,
    ISourceReader SourceReader,
    MigrationScopeProvider ScopeProvider,
    string? CurrentVersion,
    IList<SourceHeader> SourceTargets,
    IDictionary<string, string> Tokens,
    IDictionary<string, string> Metadata,
    LogLevel StatementLogLevel)
{
    
}