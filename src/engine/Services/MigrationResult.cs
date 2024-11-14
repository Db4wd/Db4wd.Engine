using DbForward.Constants;
using DbForward.Models;

namespace DbForward.Services;

public record MigrationResult(
    OperationResponse Response,
    SourceHeader[] AppliedSources,
    SourceHeader? FailedSource = null)
{
    /// <inheritdoc />
    public override string ToString() => $"{Response}";
}