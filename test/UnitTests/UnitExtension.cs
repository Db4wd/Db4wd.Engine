using System.Text;
using System.Text.Json;
using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using DbForward.Services;

namespace UnitTests;

public class UnitExtension(UnitDatabase? db = null) : IDatabaseExtension
{
    private readonly UnitDatabase database = db ?? new UnitDatabase();
    
    /// <inheritdoc />
    public string ToolName => "test";

    /// <inheritdoc />
    public string DefaultFileExtension => ".sql";

    /// <inheritdoc />
    public Task<bool> IsSchemaInitializedAsync(CancellationToken cancellationToken) => Task.FromResult(true);

    /// <inheritdoc />
    public Task<SchemaInitialization> InitializeMigrationsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(SchemaInitialization.NotRequired);
    }

    /// <inheritdoc />
    public ISourceReader GetSourceReader() => ConventionalSourceReader.Instance;

    /// <inheritdoc />
    public async Task<IMigrationScope> CreateMigrationScopeAsync(SourceOperation operation,
        OperationTracker operationTracker,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return new UnitMigrationScope(database);
    }

    /// <inheritdoc />
    public async Task WriteTemplateSource(
        TextWriter textWriter, 
        string dbVersionId,
        IReadOnlyDictionary<string, string> metadata,
        CancellationToken cancellationToken)
    {
        var builder = new StringBuilder();
        builder.AppendLine("-- [head]");
        builder.AppendLine("-- [id: 317d8644-817d-4b19-8531-3e525b2f0001]");
        builder.AppendLine($"-- [dbVersion: {dbVersionId}]");
        foreach (var (k, v) in metadata)
        {
            builder.AppendLine($"-- [metadata.{k}: {v}]");
        }
        builder.AppendLine("-- [/head]");
        builder.AppendLine();
        builder.AppendLine("-- [up]");
        builder.AppendLine("-- [/up]");
        builder.AppendLine();
        builder.AppendLine("-- [down]");
        builder.AppendLine("-- [/down]");
        
        await textWriter.WriteLineAsync(builder.ToString());
    }

    /// <inheritdoc />
    public async Task WriteTemplateEnvironmentFileAsync(TextWriter textWriter,
        Dictionary<string, string> properties,
        CancellationToken cancellationToken)
    {
        properties.TryAdd("host", "localhost");
        properties.TryAdd("database", "testdb");
        properties.TryAdd("user", "root");
        properties.TryAdd("password", "secret");

        await textWriter.WriteLineAsync(JsonSerializer.Serialize(properties, JsonOptions.Default));
    }

    /// <inheritdoc />
    public async Task<IMetadataContext> CreateMetadataContextAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return new UnitMetadataContext(database);
    }
}