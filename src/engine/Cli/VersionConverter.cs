using Vertical.Cli.Conversion;

namespace Db4Wd.Cli;

public sealed class VersionConverter : ValueConverter<Version>
{
    /// <inheritdoc />
    public override Version Convert(string s)
    {
        if (Version.TryParse(s, out var version))
            return version;

        throw new FormatException("Invalid version format '{s}', expected <major>.<minor>.<revision>");
    }
}