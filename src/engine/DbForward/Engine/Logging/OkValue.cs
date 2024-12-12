namespace DbForward.Engine.Logging;

public sealed class OkValue(string value)
{
    public static readonly OkValue Default = new("OK");
    
    /// <inheritdoc />
    public override string ToString() => value;
}