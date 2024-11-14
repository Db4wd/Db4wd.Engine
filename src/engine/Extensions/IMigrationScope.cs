using DbForward.Constants;
using DbForward.Models;
using DbForward.Services;

namespace DbForward.Extensions;

/// <summary>
/// Represents a specific database context that exposes operations for
/// an execution of a migration source's set of directives.
/// </summary>
public interface IMigrationScope : IAsyncDisposable
{
    /// <summary>
    /// Applies a directive.
    /// </summary>
    /// <param name="directive">Directive to apply</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns <see cref="OperationResponse"/></returns>
    Task<OperationResponse> ApplyDirectiveAsync(StatementSectionDirective directive,
        CancellationToken cancellationToken);

    /// <summary>
    /// Indicates all source directives have been applied, changes should be permanently
    /// committed, and the migration metadata should be saved.
    /// </summary>
    /// <param name="detail">An object that describes the migration.</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task</returns>
    Task CommitChangesAsync(MigrationOperationDetail detail, CancellationToken cancellationToken);

    /// <summary>
    /// Indicates changes should be discard, the transaction aborted, etc.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation.</param>
    /// <returns><see cref="OperationResponse"/></returns>
    Task<OperationResponse> DiscardChangesAsync(CancellationToken cancellationToken);
}