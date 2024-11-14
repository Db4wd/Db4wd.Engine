namespace DbForward.Features;

/// <summary>
/// Defines a feature host.
/// </summary>
public interface IFeatureHost
{
    /// <summary>
    /// Executes the feature.
    /// </summary>
    /// <param name="options">Runtime options</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <typeparam name="TOptions">Options type</typeparam>
    /// <returns>Task that returns <see cref="int"/></returns>
    Task<int> ExecuteFeatureAsync<TOptions>(TOptions options, CancellationToken cancellationToken)
        where TOptions : GlobalOptions;
}