namespace DbForward.Engine;

public interface IEngineHost
{
    /// <summary>
    /// Executes the engine.
    /// </summary>
    /// <param name="arguments">Arguments received from input.</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns the <c>int</c> status code.s</returns>
    Task<int> ExecuteAsync(string[] arguments, CancellationToken cancellationToken = default);
}