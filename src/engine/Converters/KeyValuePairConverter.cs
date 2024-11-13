using System.Text.RegularExpressions;
using Vertical.Cli.Conversion;

namespace DbForward.Converters;

internal sealed partial class KeyValuePairConverter : ValueConverter<KeyValuePair<string, string>>
{
    /// <inheritdoc />
    public override KeyValuePair<string, string> Convert(string s)
    {
        if (MyRegex().Match(s) is { Success: true } match)
        {
            return new KeyValuePair<string, string>(
                match.Groups["key"].Value,
                match.Groups["value"].Value);
        }

        throw new FormatException($"Invalid key/value pair format '{s}'");
    }

    [GeneratedRegex(@"(?<key>[\w\d_-]+)=(?<value>.+)")]
    private static partial Regex MyRegex();
}