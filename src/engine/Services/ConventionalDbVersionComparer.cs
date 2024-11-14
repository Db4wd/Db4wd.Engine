namespace DbForward.Services;

public sealed class ConventionalDbVersionComparer : IDbVersionComparer
{
    public static IDbVersionComparer Instance { get; } = new ConventionalDbVersionComparer();
    
    public int Compare(string? x, string? y) => string.Compare(x, y, StringComparison.Ordinal);

    public static string CreateDbVersion(DateTime serverTime) => serverTime.ToString("s");

    /// <inheritdoc />
    public bool IsValid(string version) => DateTime.TryParse(version, out _);
}