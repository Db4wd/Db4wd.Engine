namespace DbForward.Constants;

/// <summary>
/// Defines the source operations
/// </summary>
public enum SourceOperation
{
    /// <summary>
    /// Indicates migration actions should be performed
    /// </summary>
    Migrate,
    
    /// <summary>
    /// Indicates rollback operations should be performed
    /// </summary>
    Rollback
}