using DbForward.Models;

namespace DbForward.Extensions;

public interface IMetadataContext : IAsyncDisposable
{
    /// <summary>
    /// Gets the currently applied migration entries.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns a list of <see cref="MigrationEntry"/> objects</returns>
    Task<IList<MigrationEntry>> GetEntriesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets detail for a migration id.
    /// </summary>
    /// <param name="migrationId">Migration id</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns a <see cref="MigrationDetail"/> if found, otherwise <c>null</c></returns>
    Task<MigrationDetail?> GetDetailAsync(Guid migrationId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the detail for the most currently applied migration.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns a <see cref="MigrationDetail"/> if found, otherwise <c>null</c></returns>
    Task<MigrationDetail?> GetCurrentDetailAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the current version.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancalltion</param>
    /// <returns>Version string or <c>null</c></returns>
    Task<string?> GetCurrentVersionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the blob applied for a migration.
    /// </summary>
    /// <param name="migrationId">Migration id</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns <see cref="CompressedBlobInfo"/> or <c>null</c></returns>
    Task<CompressedBlobInfo?> GetBlobAsync(Guid migrationId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets log entries from the most recent backward.
    /// </summary>
    /// <param name="parameters">Parameters used to search logs</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns a list of <see cref="MigrationHistory"/> objects.</returns>
    Task<IList<MigrationHistory>> GetLogEntriesAsync(LogSearchParameters parameters, 
        CancellationToken cancellationToken);
}