using System.Collections.ObjectModel;
using Dapper;
using DbForward.Constants;
using DbForward.Models;
using Npgsql;

namespace DbForward.Postgres.Commands;

internal static class SaveMigrationDetail
{
    internal static async Task ExecuteAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        MigrationOperationDetail detail)
    {
        var logId = Guid.NewGuid();
        await SaveLogDataAsync(connection, transaction, detail, logId);

        switch (detail.Operation)
        {
            case SourceOperation.Migrate:
                await SaveMigrationDataAsync(connection, transaction, detail, logId);
                break;

            case SourceOperation.Rollback:
            default:
                await DeleteMigrationEntryAsync(connection, transaction, detail.MigrationId);
                break;
        }
    }
    
    private static async Task SaveLogDataAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        MigrationOperationDetail detail,
        Guid logId)
    {
        const string schema = Constants.SchemaName;

        await connection.ExecuteAsync(
            $"""
             insert into {schema}.log(logid, migrationid, dateapplied, operation, agent, host)
             values(@logId, @MigrationId, CURRENT_TIMESTAMP, @Operation, @Agent, @Host);
             """,
            new
            {
                logId,
                detail.MigrationId,
                Operation = detail.Operation.ToString(),
                detail.Agent,
                detail.Host
            },
            transaction: transaction);
    }
    
    private static async Task SaveMigrationDataAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        MigrationOperationDetail detail,
        Guid logId)
    {
        const string schema = Constants.SchemaName;

        var parameters = new
        {
            logId,
            detail.MigrationId,
            detail.DbVersion,
            detail.SourcePath,
            detail.SourceFile,
            detail.BlobInfo?.Sha,
            detail.BlobInfo?.Compression,
            detail.BlobInfo?.Encoding,
            detail.BlobInfo?.SourceLength,
            detail.BlobInfo?.CompressedBytes
        };
        
        await connection.ExecuteAsync(
            $"""
             insert into {schema}.migrations(migrationid, dbversion, logid, sourcepath, sourcefile, sha)
             values(@MigrationId, @DbVersion, @logId, @SourcePath, @SourceFile, @Sha);             
             """,
            parameters,
            transaction);

        await connection.ExecuteAsync(
            $"""
             insert into {schema}.blobs(migrationid, compression, encoding, content, contentlength)
             values(@MigrationId, @Compression, @Encoding, @CompressedBytes, @SourceLength);
             """,
            parameters,
            transaction);

        foreach (var (key, value) in detail.Metadata ?? ReadOnlyDictionary<string, string>.Empty)
        {
            await connection.ExecuteAsync(
                $"""
                 insert into {schema}.metadata(migrationid, key, value)
                 values(@id, @key, @value); 
                 """,
                new { id = detail.MigrationId, key, value },
                transaction);
        }

        foreach (var (key, value) in detail.OperationTracker?.Metrics ?? ReadOnlyDictionary<string, long>.Empty)
        {
            await connection.ExecuteAsync(
                $"""
                 insert into {schema}.metrics(migrationid, category, key, value)
                 values(@id, 'metric', @key, @value);
                 """,
                new { id = detail.MigrationId, key, value = value.ToString() },
                transaction);
        }

        foreach (var (key, value) in detail.OperationTracker?.Tags ?? ReadOnlyDictionary<string, string>.Empty)
        {
            await connection.ExecuteAsync(
                $"""
                 insert into {schema}.metrics(migrationid, category, key, value)
                 values(@id, 'tag', @key, @value);
                 """,
                new { id = detail.MigrationId, key, value },
                transaction);
        }
    }

    private static async Task DeleteMigrationEntryAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        Guid migrationId)
    {
        const string schema = Constants.SchemaName;
        
        await connection.ExecuteAsync(
            $"""
             delete from {schema}.migrations
             where migrationid = @migrationId;
             """,
            new { migrationId },
            transaction: transaction);
    }
}