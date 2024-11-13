using DbForward.Constants;
using DbForward.Models;

namespace DbForward.Services;

public record MigrationResult(
    OperationResponse Response,
    SourceHeader[] AppliedSources,
    string? CurrentVersion,
    SourceHeader? FailedSource = null)
{
    /// <inheritdoc />
    public override string ToString() => $"{Response}";
}