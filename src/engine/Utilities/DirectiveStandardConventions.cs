using System.Text.RegularExpressions;

namespace Db4Wd.Utilities;

public static partial class DirectiveStandardConventions
{
    public static bool TryGetMigrationId(string str, out Guid id)
    {
        id = IdRegex().Match(str) is { Success: true } match
            ? Guid.Parse(match.Groups[1].Value)
            : Guid.Empty;

        return id != Guid.Empty;
    }

    public static bool TryGetDbVersionId(string str, out int version)
    {
        version = VersionRegex().Match(str) is { Success: true } match
            ? int.Parse(match.Groups[1].Value)
            : 0;

        return version > 0;
    }

    public static bool TryGetMetadataPair(string str, out KeyValuePair<string, string> metadata)
    {
        metadata = Regex.Match(str, @"^\{metadata.([\w\d_-]+)=([^}]+)}") is { Success: true } match
            ? new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value)
            : default;

        return !string.IsNullOrWhiteSpace(metadata.Key);
    }

    [GeneratedRegex(@"^\{id:\s([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\}")]
    private static partial Regex IdRegex();
    
    [GeneratedRegex(@"^\{dbVersion:\s(\d+)\}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex VersionRegex();
}