namespace Db4Wd.Engine;

public sealed class AgentContext(TimeSpan timeZoneOffset)
{
    public TimeSpan TimeZoneOffset => timeZoneOffset;
    
    public string Agent { get; } = TryGetValue(
        () => Environment.UserName,
        "(unavailable)");

    public string Host { get; } = TryGetValue(
        () => Environment.MachineName,
        "(unavailable)");
    
    private static string TryGetValue(Func<string> provider, string defaultValue)
    {
        try
        {
            return provider();
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <inheritdoc />
    public override string ToString() => $"{Host}/{Agent}";
}