namespace DbForward.Features;

/// <summary>
/// Handles feature implementations.
/// </summary>
/// <typeparam name="TOptions">Options type</typeparam>
public interface IFeature<in TOptions> where TOptions : GlobalOptions
{
    /// <summary>
    /// Handles the feature's implementation.
    /// </summary>
    /// <param name="options">Options parsed from arguments.</param>
    /// <param name="cancellationToken">Token observed for cancellation.</param>
    /// <returns>Task that returns an <c>int</c> status code.</returns>
    Task<int> HandleAsync(TOptions options, CancellationToken cancellationToken);
}