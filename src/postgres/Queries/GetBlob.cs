using Dapper;
using DbForward.Models;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal sealed class GetBlob
{
    internal static async Task<CompressedBlobInfo?> QueryAsync(NpgsqlConnection connection, Guid id)
    {
        const string sql =
            $"""
             select m.sourcepath, m.sourcefile, m.sha,
                    b.compression, b.encoding, b.content, b.contentlength
             from {Constants.SchemaName}.migrations_view m
             join {Constants.SchemaName}.blobs b on (b.migrationid = m.migrationid)
             where {Constants.SchemaName}.is_uuid_match(m.migrationid, @id);             
             """;

        var result = await connection.QuerySingleOrDefaultAsync(sql, new { id });

        if (result == null)
            return null;

        return new CompressedBlobInfo(
            Path.Combine(result.sourcepath, result.sourcefile),
            result.contentlength,
            result.sha,
            result.compression,
            result.encoding,
            result.content);
    }
}