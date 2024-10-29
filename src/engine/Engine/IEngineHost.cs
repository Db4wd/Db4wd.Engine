namespace Db4Wd.Engine;

public interface IEngineHost
{
    Task<int> ExecuteAsync(string[] arguments, CancellationToken cancellationToken = default);
}