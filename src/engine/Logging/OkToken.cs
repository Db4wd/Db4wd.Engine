namespace Db4Wd.Logging;

public sealed class OkToken(string value)
{
    /// <inheritdoc />
    public override string ToString() => value;

    public static OkToken Default { get; } = new("OK:");
}