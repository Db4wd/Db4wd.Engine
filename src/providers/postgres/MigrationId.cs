using System.Text.RegularExpressions;

namespace DbForward.Postgres;

internal static partial class MigrationId
{
    public static bool TryParse(string str, out string? id)
    {
        id = default;

        if (IdRegex().Match(str) is not { Success: true } match)
            return false;

        id = match.Groups[1].Value;
        return true;
    }

    public static string Create()
    {
        var guid = Guid.NewGuid().ToString()[^8..].ToLower();
        var now = DateTime.Now;

        return $"{now:yyyyMMdd}_{now:hhmmss}_{guid}";
    }

    [GeneratedRegex(@"^-- \[id: (\d{8}_\d{6}_[0-9a-f]{8})]$")]
    private static partial Regex IdRegex();
}