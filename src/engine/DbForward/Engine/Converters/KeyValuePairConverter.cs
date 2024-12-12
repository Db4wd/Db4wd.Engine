using System.Text.RegularExpressions;
using Vertical.Cli.Conversion;

namespace DbForward.Engine.Converters;

public sealed partial class KeyValuePairConverter : ValueConverter<KeyValuePair<string, string>>
{
    /// <inheritdoc />
    public override KeyValuePair<string, string> Convert(string str)
    {
        if (MyRegex().Match(str) is { Success: true } match)
        {
            return new KeyValuePair<string, string>(
                match.Groups[1].Value,
                match.Groups[2].Value);
        }

        throw new FormatException("invalid key/value format");
    }

    [GeneratedRegex(@"^(\w[\w\d_-]*)=([\S]+)")]
    private static partial Regex MyRegex();
}