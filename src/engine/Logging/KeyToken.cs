namespace Db4Wd.Logging;

public sealed class KeyToken(string value)
{
    /// <inheritdoc />
    public override string ToString() => value;
}