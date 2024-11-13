using System.Text.RegularExpressions;
using DbForward.Services;

namespace DbForward.Postgres;

internal sealed partial class PostgresDbVersionComparer : IDbVersionComparer
{
    public int Compare(string? x, string? y) => string.Compare(x, y, StringComparison.Ordinal);

    /// <inheritdoc />
    public bool IsValid(string version) => MyRegex().IsMatch(version);
    
    [GeneratedRegex(@"^[\d]{4}-[\d]{2}-[\d]{2}.[\d]{5}$")]
    private static partial Regex MyRegex();
}