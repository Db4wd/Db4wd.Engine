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
    /// Gets the history for a particular migration.
    /// </summary>
    /// <param name="migrationId">Migrationid</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns a list of <see cref="MigrationHistory"/> objects.</returns>
    Task<IList<MigrationHistory>> GetHistoryAsync(Guid migrationId, int limit, CancellationToken cancellationToken);

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
    /// Gets the blob applied for a migration.
    /// </summary>
    /// <param name="migrationId">Migration id</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns <see cref="CompressedBlobInfo"/> or <c>null</c></returns>
    Task<CompressedBlobInfo?> GetBlobAsync(Guid migrationId, CancellationToken cancellationToken);
}