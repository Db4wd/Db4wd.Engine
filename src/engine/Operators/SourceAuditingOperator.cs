using Db4Wd.Models;
using Db4Wd.Parsing;
using Db4Wd.Utilities;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Operators;

public sealed class SourceAuditingOperator(ILogger<SourceAuditingOperator> logger)
{
    /// <summary>
    /// Audits a source file input set.
    /// </summary>
    /// <param name="sourceFiles">Input set</param>
    /// <param name="migrationEntries">Previously applied migrations</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    public async Task AuditSourcesAsync(
        SourceFileHeader[] sourceFiles,
        MigrationEntry[] migrationEntries, 
        CancellationToken cancellationToken)
    {
        // Verify all previous applied migrations source files are in source set
        await ValidateAppliedMigrationSourcesAsync(sourceFiles, migrationEntries, cancellationToken);

        var entryIdDictionary = migrationEntries.ToDictionary(entry => entry.Id);
        var pendingSources = sourceFiles.Where(source => !entryIdDictionary.ContainsKey(source.MigrationId)).ToArray();
        
        // Verify no source files are inserted out of version order
        ValidatePendingSourcesDbVersionOrder(pendingSources, entryIdDictionary);
        
        // Verify db versions and migrations are unique in pending sources
        ValidatePendingSourceIdentifiers(pendingSources);
    }

    private void ValidatePendingSourcesDbVersionOrder(
        SourceFileHeader[] pendingSources, 
        Dictionary<Guid, MigrationEntry> entryIdDictionary)
    {
        if (pendingSources.Length == 0 || entryIdDictionary.Count == 0)
            return;

        var maxDbVersion = entryIdDictionary.Values.Max(entry => entry.DbVersion);

        var throwError = false;
        
        foreach (var source in pendingSources.Where(source => source.DbVersion < maxDbVersion))
        {
            logger.LogError("Source {path} has lower version than most recent applied migration (version={version})",
                source.Context,
                maxDbVersion);
            
            throwError = true;
        }

        if (!throwError)
            return;

        throw ValidationFailed;
    }

    private async Task ValidateAppliedMigrationSourcesAsync(
        SourceFileHeader[] sourceFiles,
        MigrationEntry[] migrationEntries, 
        CancellationToken cancellationToken)
    {
        if (migrationEntries.Length == 0)
            return;
        
        var pathDictionary = sourceFiles.ToDictionary(source => Path.GetFileName(source.Context));
        var throwError = false;

        foreach (var entry in migrationEntries)
        {
            if (!pathDictionary.TryGetValue(entry.Filename, out var source))
            {
                logger.LogError("Applied source {path} missing from input set", entry.Filename);
                throwError = true;
            }

            if (!await Hashing.VerifyAsync(() => File.OpenRead(source!.Context), entry.Sha, cancellationToken))
            {
                logger.LogError("Source {path} content changed since migration was applied (hashes differ)",
                    source!.Context);
                throwError = true;
            }
            
            logger.LogDebug("Validated source {path} (sha={sha})", source!.Context, entry.Sha);
        }

        if (!throwError)
            return;

        throw ValidationFailed;
    }

    private void ValidatePendingSourceIdentifiers(SourceFileHeader[] pendingSources)
    {
        if (pendingSources.Length == 0)
            return;
        
        var idDictionary = new Dictionary<Guid, SourceFileHeader>();
        var dbVersionDictionary = new Dictionary<int, SourceFileHeader>();
        var throwError = false;
        
        foreach (var source in pendingSources)
        {
            if (!idDictionary.TryAdd(source.MigrationId, source))
            {
                logger.LogError("Source {path} migration id {id} conflicts with {otherPath}",
                    source.Context,
                    source.MigrationId,
                    idDictionary[source.MigrationId].Context);
                
                throwError = true;
            }

            if (dbVersionDictionary.TryAdd(source.DbVersion, source))
                continue;
            
            logger.LogError("Source {path} dbVersionId {id} conflicts with {ptherPath}",
                source.Context,
                source.MigrationId,
                dbVersionDictionary[source.DbVersion].Context);
            
            throwError = true;
        }

        if (!throwError)
            return;

        throw ValidationFailed;
    }

    private static ApplicationException ValidationFailed => new("Failed to validate source input set");
}