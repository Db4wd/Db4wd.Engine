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
    public IDbVersionComparer GetDbVersionComparer() => ConventionalDbVersionComparer.Instance;

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
        IReadOnlyDictionary<string, string> metadata,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task WriteTemplateEnvironmentFileAsync(TextWriter textWriter,
        Dictionary<string, string> properties,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<IMetadataContext> CreateMetadataContextAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return new UnitMetadataContext(database);
    }

    /// <inheritdoc />
    public Task PostCheckStateAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}