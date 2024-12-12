namespace DbForward.Engine.Logging;

public sealed class InfoValue(string value)
{
    /// <inheritdoc />
    public override string ToString() => value;
}