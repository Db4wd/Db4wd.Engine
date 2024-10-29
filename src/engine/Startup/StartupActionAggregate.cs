namespace Db4Wd.Startup;

public sealed class StartupActionAggregate(IEnumerable<IStartupAction> startupActions)
{
    public async Task PerformAllAsync()
    {
        foreach (var action in startupActions)
        {
            await action.PerformAsync();
        }
    }
}