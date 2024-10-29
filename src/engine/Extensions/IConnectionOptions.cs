namespace Db4Wd.Extensions;

/// <summary>
/// Defines general parameters common to database drivers.
/// </summary>
public interface IConnectionOptions
{
    /// <summary>
    /// Gets the connection string.
    /// </summary>
    string? ConnectionString { get; }
    
    /// <summary>
    /// Gets the host.
    /// </summary>
    string? Host { get; }
    
    /// <summary>
    /// Gets the port.
    /// </summary>
    uint? Port { get; }
    
    /// <summary>
    /// Gets the database name.
    /// </summary>
    string? Database { get; }
    
    /// <summary>
    /// Gets the user id.
    /// </summary>
    string? UserId { get; }
    
    /// <summary>
    /// Gets the password.
    /// </summary>
    string? Password { get; }
    
    /// <summary>
    /// Gets a dictionary of additional properties.
    /// </summary>
    Dictionary<string, string> Properties { get; }
}