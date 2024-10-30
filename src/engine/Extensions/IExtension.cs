namespace Db4Wd.Extensions;

/// <summary>
/// Primary interface for applications that extend the DB4wd engine.
/// </summary>
public interface IExtension
{
    /// <summary>
    /// Gets the extensions root context.
    /// </summary>
    string RootContext { get; }
    
    /// <summary>
    /// Gets the default extension of migration files.
    /// </summary>
    string DefaultFileExtension { get; }
    
    /// <summary>
    /// Gets any properties that should be displayed by the version feature.
    /// </summary>
    IReadOnlyDictionary<string, string> Properties { get; }

    /// <summary>
    /// Initializes the connector.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation.</param>
    /// <returns><see cref="ConnectorProperties"/></returns>
    Task<ConnectorProperties> InitializeAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Installs or updates the schema management version.
    /// </summary>
    /// <param name="connectorProperties">Previously obtained connector properties.</param>
    /// <param name="version">Version to install or update to</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    Task<ConnectorInstallation> UpdateManagementVersionAsync(
        ConnectorProperties connectorProperties,
        Version version,
        CancellationToken cancellationToken);

    /// <summary>
    /// Writes a template environment file
    /// </summary>
    /// <param name="writer">Text writer that represents the output stream</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task</returns>
    Task WriteEnvironmentTemplateAsync(TextWriter writer, CancellationToken cancellationToken);
}