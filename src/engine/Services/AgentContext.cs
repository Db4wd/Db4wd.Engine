namespace DbForward.Services;

public interface IAgentContext
{
    string Agent { get; }
    
    string Host { get; }
    
    TimeSpan TimeZoneOffset { get; }
}

internal sealed class DefaultAgentContext : IAgentContext
{
    /// <inheritdoc />
    public string Agent { get; } = TryGetValue(() => Environment.UserName, "n/a");

    /// <inheritdoc />
    public string Host { get; } = TryGetValue(() => Environment.MachineName, "n/a");

    /// <inheritdoc />
    public TimeSpan TimeZoneOffset { get; } = TimeZoneInfo.Local.BaseUtcOffset;

    private static string TryGetValue(Func<string> func, string defaultValue)
    {
        try
        {
            return func();
        }
        catch
        {
            return defaultValue;
        }
    }
}