using Vertical.Cli.Conversion;

namespace DbForward.Converters;

public sealed class PartialGuidConverter : ValueConverter<Guid>
{
    /// <inheritdoc />
    public override Guid Convert(string s)
    {
        return s.Length switch
        {
            36 => Guid.Parse(s),
            8 => Guid.Parse($"00000000-0000-0000-0000-0000{s}"),
            _ => throw new FormatException("Value is not a valid full or partial GUID")
        };
    }
}

public sealed class PartialNullableGuidConverter : ValueConverter<Guid?>
{
    /// <inheritdoc />
    public override Guid? Convert(string s)
    {
        return s.Length switch
        {
            36 => Guid.Parse(s),
            8 => Guid.Parse($"00000000-0000-0000-0000-0000{s}"),
            _ => throw new FormatException("Value is not a valid full or partial GUID")
        };
    }
}