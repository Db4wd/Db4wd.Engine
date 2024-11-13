namespace DbForward.Constants;

public enum OperationResponse
{
    /// <summary>
    /// Indicates no rollback is required
    /// </summary>
    Successful,
    
    /// <summary>
    /// Indicates the operation was aborted due to an error, but no
    /// further action is required
    /// </summary>
    Aborted,
    
    /// <summary>
    /// Indicates the operation was aborted due to an error, and rollback
    /// actions should be initiated
    /// </summary>
    Rollback
}