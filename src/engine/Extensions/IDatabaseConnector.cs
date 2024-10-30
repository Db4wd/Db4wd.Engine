using Db4Wd.Models;
using Db4Wd.Utilities;

namespace Db4Wd.Extensions;

public interface IDatabaseConnector
{
    /// <summary>
    /// Gets active locks pending in the target database.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns><see cref="LockInfo"/> collection</returns>
    Task<IReadOnlyCollection<LockInfo>> GetLocksAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Attempts to release a lock.
    /// </summary>
    /// <param name="id">Lock id</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns><c>bool</c></returns>
    Task<LockResult> TryReleaseLockAsync(Guid? id, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an object used to parse migration sources.
    /// </summary>
    /// <returns><see cref="IMigrationSourceReader"/></returns>
    IMigrationSourceReader CreateSourceReader();

    /// <summary>
    /// Gets all migration entries.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns><see cref="MigrationEntry"/> collection</returns>
    Task<IReadOnlyCollection<MigrationEntry>> GetMigrationEntriesAsync(CancellationToken cancellationToken);
}