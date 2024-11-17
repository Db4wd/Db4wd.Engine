namespace DbForward.Utilities;

public sealed class PartialGuidEqualityComparer : IEqualityComparer<Guid?>
{
    private PartialGuidEqualityComparer(){}

    public static readonly IEqualityComparer<Guid?> Default = new PartialGuidEqualityComparer();
    
    private static bool AreEqual(Guid? x, Guid? y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        if (x == y)
        {
            return true;
        }

        var (xBytes, yBytes) = (x.Value.ToByteArray(), y.Value.ToByteArray());

        return (xBytes is [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, ..] || yBytes is [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, ..])
               && xBytes[^4..].SequenceEqual(yBytes[^4..]);
    }

    /// <inheritdoc />
    public bool Equals(Guid? x, Guid? y) => AreEqual(x, y);

    /// <inheritdoc />
    public int GetHashCode(Guid? obj) => obj?.GetHashCode() ?? 0;
}