using DbForward.Constants;
using DbForward.Models;
using DbForward.Services;

namespace DbForward.Extensions;

/// <summary>
/// Interface implemented by extensions.
/// </summary>
public interface IDatabaseExtension
{
    /// <summary>
    /// Gets the name of the tool as invoked.
    /// </summary>
    string ToolName { get; }
    
    /// <summary>
    /// Gets the default search pattern.
    /// </summary>
    string DefaultFileExtension { get; }

    /// <summary>
    /// Gets the an object used to comparer database version.
    /// </summary>
    /// <returns><see cref="IDbVersionComparer"/></returns>
    IDbVersionComparer GetDbVersionComparer();

    /// <summary>
    /// Gets whether the migration schema is initialized.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns><c>bool</c></returns>
    Task<bool> IsSchemaInitializedAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Initializes migrations in the target database.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation.</param>
    /// <returns><see cref="Task"/></returns>
    Task<SchemaInitialization> InitializeMigrationsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a <see cref="ISourceReader"/> capable of reading database specific source files.
    /// </summary>
    /// <returns><see cref="ISourceReader"/></returns>
    ISourceReader GetSourceReader();

    /// <summary>
    /// Creates a scoped migration context.
    /// </summary>
    /// <param name="operation">The operation being performed.</param>
    /// <param name="operationTracker">Tracking data for the operation</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns a <see cref="IMigrationScope"/></returns>
    Task<IMigrationScope> CreateMigrationScopeAsync(SourceOperation operation,
        OperationTracker operationTracker,
        CancellationToken cancellationToken);

    /// <summary>
    /// Writes template content for a new migration source.
    /// </summary>
    /// <param name="textWriter">Text writer to write the template to</param>
    /// <param name="metadata">Metadata to include in the template</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task</returns>
    Task WriteTemplateSource(TextWriter textWriter,
        IReadOnlyDictionary<string, string> metadata,
        CancellationToken cancellationToken);

    /// <summary>
    /// Writes a template environment file.
    /// </summary>
    /// <param name="textWriter">Text writer to write the template to</param>
    /// <param name="properties">Properties to set in the file</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task</returns>
    Task WriteTemplateEnvironmentFileAsync(TextWriter textWriter,
        Dictionary<string, string> properties,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a metadata context.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns an <see cref="IMetadataContext"/></returns>
    Task<IMetadataContext> CreateMetadataContextAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Called after a migration or rollback operation.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task</returns>
    Task PostCheckStateAsync(CancellationToken cancellationToken);
}