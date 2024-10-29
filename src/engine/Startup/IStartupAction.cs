namespace Db4Wd.Startup;

public interface IStartupAction
{
    int Priority { get; }
    
    Task PerformAsync();
}