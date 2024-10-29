using System.Text.RegularExpressions;
using Vertical.Cli.Conversion;

namespace Db4Wd.Cli;

public sealed partial class KeyValuePairConverter : ValueConverter<KeyValuePair<string, string>>
{
    /// <inheritdoc />
    public override KeyValuePair<string, string> Convert(string s)
    {
        if (MyRegex().Match(s) is not { Success: true } match)
        {
            throw new FormatException($"Invalid key/value pair format '{s}'");
        }

        return new KeyValuePair<string, string>(match.Groups["key"].Value, match.Groups["value"].Value);
    }

    [GeneratedRegex(@"^(?<key>[\w\d_-]+)=(?<value>.+)")]
    private static partial Regex MyRegex();
}