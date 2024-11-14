using System.Collections.ObjectModel;
using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using DbForward.Services;

namespace UnitTests;

public class UnitMetadataContext(UnitDatabase database) : IMetadataContext
{
    /// <inheritdoc />
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    /// <inheritdoc />
    public async Task<IList<MigrationEntry>> GetEntriesAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        return database
            .Migrations
            .Where(op => op.Detail.Operation == SourceOperation.Migrate)
            .Select(op => new MigrationEntry(
                op.Detail.MigrationId,
                op.Detail.DbVersion,
                op.Date,
                op.Detail.SourcePath,
                op.Detail.SourceFile,
                op.Detail.BlobInfo!.Sha))
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<IList<MigrationHistory>> GetHistoryAsync(
        Guid migrationId, 
        int limit, 
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        return database
            .Migrations
            .Where(op => op.Detail.MigrationId == migrationId)
            .Select(op => new MigrationHistory(
                op.Detail.MigrationId,
                op.LogId,
                op.Date,
                op.Detail.Operation.ToString(),
                op.Detail.Agent,
                op.Detail.Host))
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<MigrationDetail?> GetDetailAsync(Guid migrationId, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var entry = database.Migrations.FirstOrDefault(e => e.Detail.MigrationId == migrationId);
        var detail = entry?.Detail;
        return detail == null
            ? null
            : new MigrationDetail(
                migrationId,
                detail.DbVersion,
                entry!.Date,
                entry.LogId,
                detail.SourcePath,
                detail.SourceFile,
                detail.BlobInfo!.Sha,
                detail.Agent,
                detail.Host,
                new Dictionary<string, string>(detail.OperationTracker!.Metrics
                    .Select(m => new KeyValuePair<string, string>(m.Key, m.Value.ToString()))),
                new Dictionary<string, string>(detail.OperationTracker!.Tags),
                detail.Metadata ?? ReadOnlyDictionary<string, string>.Empty);
    }

    /// <inheritdoc />
    public async Task<MigrationDetail?> GetCurrentDetailAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var migration = database
            .Migrations
            .MaxBy(m => m.Detail.DbVersion, StringComparer.Ordinal);

        return migration == null 
            ? null 
            : await GetDetailAsync(migration.Detail.MigrationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> GetCurrentVersionAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return database
            .Migrations
            .Select(op => op.Detail.DbVersion)
            .Max(ConventionalDbVersionComparer.Instance);
    }

    /// <inheritdoc />
    public async Task<CompressedBlobInfo?> GetBlobAsync(Guid migrationId, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var entry = database.Migrations.FirstOrDefault(e => e.Detail.MigrationId == migrationId);

        return entry == null
            ? null
            : new CompressedBlobInfo(
                Path.Combine(entry.Detail.SourcePath, entry.Detail.SourceFile),
                entry.Detail.BlobInfo!.SourceLength,
                entry.Detail.BlobInfo!.Sha,
                entry.Detail.BlobInfo!.Compression,
                entry.Detail.BlobInfo!.Encoding,
                entry.Detail.BlobInfo!.CompressedBytes);
    }
}