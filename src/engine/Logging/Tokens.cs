namespace DbForward.Logging;

public sealed class OkToken(string value = "Ok")
{
    /// <inheritdoc />
    public override string ToString() => value;

    public static readonly OkToken Default = new();
}

public sealed class VerboseToken(string value)
{
    /// <inheritdoc />
    public override string ToString() => value;
}