using Vertical.Cli.Configuration;

namespace Db4Wd.Cli;

public sealed class CliSymbolComparer : IComparer<ICliSymbol>
{
    /// <inheritdoc />
    public int Compare(ICliSymbol? x, ICliSymbol? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (ReferenceEquals(x, null))
            return 1;

        if (ReferenceEquals(y, null))
            return -1;

        return string.Compare(x.Names[0], y.Names[0], StringComparison.Ordinal);
    }
}