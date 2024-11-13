using System.Text.Json;
using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using DbForward.Postgres.Queries;
using DbForward.Services;
using DbForward.Utilities;

namespace DbForward.Postgres;

public sealed class PostgresDatabaseExtensions(
    IConnectionFactory connectionFactory,
    ISchemaInitializer schemaInitializer,
    IMigrationScopeFactory scopeFactory,
    IFileSystem fileSystem,
    IAgentContext agentContext,
    IMetadataContext metadataContext)
    : IDatabaseExtension
{
    private readonly ISourceReader sourceReader = new PostgresSourceReader();
    private readonly IDbVersionComparer versionComparer = new PostgresDbVersionComparer();
    
    /// <inheritdoc />
    public string ToolName => Constants.ToolName;

    /// <inheritdoc />
    public string DefaultFileExtension => ".sql";

    /// <inheritdoc />
    public IDbVersionComparer GetDbVersionComparer() => versionComparer;

    /// <inheritdoc />
    public async Task<bool> IsSchemaInitializedAsync(CancellationToken cancellationToken)
    {
        return await schemaInitializer.IsSchemaInitializedAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SchemaInitialization> InitializeMigrationsAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);

        if (await schemaInitializer.InitializeAsync(this, cancellationToken))
            return SchemaInitialization.Initialized;

        throw new InvalidOperationException("Failed to initialize internal metadata schema.");
    }

    /// <inheritdoc />
    public ISourceReader GetSourceReader() => sourceReader;

    /// <inheritdoc />
    public async Task<IMigrationScope> CreateMigrationScopeAsync(SourceOperation operation,
        OperationTracker operationTracker,
        CancellationToken cancellationToken)
    {
        return await scopeFactory.CreateAsync(operation, operationTracker, cancellationToken);
    }

    /// <inheritdoc />
    public async Task WriteTemplateSource(TextWriter textWriter,
        IReadOnlyDictionary<string, string> metadata,
        CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        using var textReader = fileSystem.CreateReader(Path.Combine(
            Constants.RootAssetPath,
            "Templates",
            "template.sql"));
        
        var sequenceId = await NextSequenceId.QueryAsync(connection);
        var tokens = new Dictionary<string, string>
        {
            ["$(migrationId)"] = Guid.NewGuid().ToString(),
            ["$(dbVersion)"] = DbVersion.Create(sequenceId),
            ["$(author)"] = agentContext.Agent
        };

        var content = await textReader.ReadAllTextAsync(tokens, cancellationToken);
        var metadataStr = metadata.Any()
            ? Environment.NewLine + string.Join(Environment.NewLine,
                metadata.Select(kv => $"-- [metadata.{kv.Key}: {kv.Value}]"))
            : string.Empty;

        content = content.Replace("$(metadata)", metadataStr);

        await textWriter.WriteLineAsync(content);
    }

    /// <inheritdoc />
    public async Task WriteTemplateEnvironmentFileAsync(
        TextWriter textWriter, 
        Dictionary<string, string> properties,
        CancellationToken cancellationToken)
    {
        StubProperties(properties);

        var json = JsonSerializer.Serialize(properties, DefaultJsonOptions.Value)
                   ?? throw new InvalidOperationException();
        await textWriter.WriteLineAsync(json);
    }

    private static void StubProperties(Dictionary<string, string> properties)
    {
        if (properties.ContainsKey("connectionstring"))
            return;
        
        TryAdd("host", "localhost");
        TryAdd("port", "5432");
        TryAdd("database", "public");
        TryAdd("userid", "postgres");
        TryAdd("password", "secret");
        TryAdd("commandtimeout", "30");

        return;

        void TryAdd(string key, string value)
        {
            if (properties.ContainsKey(key))
                return;
            
            properties.Add(key, value);
        }
    }

    /// <inheritdoc />
    public Task<IMetadataContext> CreateMetadataContextAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(metadataContext);
    }
}