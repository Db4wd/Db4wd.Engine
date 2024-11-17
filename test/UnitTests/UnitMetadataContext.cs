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
    public async Task<IList<MigrationEntry>> GetEntriesWithTagAsync(KeyValuePair<string, string> tag, int limit,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return database.Migrations.Where(m => m.Detail.Metadata?.TryGetValue(tag.Key, out var value) == true
                                              && value.Contains(tag.Value))
            .Select(m => m.Detail)
            .Select(d => new MigrationEntry(
                d.MigrationId,
                d.DbVersion,
                database.Logs.First(l => l.MigrationId == d.MigrationId).DateApplied,
                d.SourcePath,
                d.SourceFile,
                d.BlobInfo!.Sha))
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

    /// <inheritdoc />
    public async Task<IList<MigrationHistory>> GetLogEntriesAsync(LogSearchParameters parameters, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        
        var logs = database.Logs.Where(log =>
            (parameters.MigrationId == null || log.MigrationId == parameters.MigrationId) &&
            (parameters.RangeStart == null || log.DateApplied >= parameters.RangeStart) &&
            (parameters.RangeEnd == null || log.DateApplied <= parameters.RangeEnd) &&
            (parameters.Operation == null || log.Operation == parameters.Operation) &&
            (parameters.Agent == null || log.Agent == parameters.Agent) &&
            (parameters.Host == null || log.Host == parameters.Host));

        return logs.ToArray();
    }
}