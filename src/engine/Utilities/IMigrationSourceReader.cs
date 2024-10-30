using Db4Wd.Parsing;

namespace Db4Wd.Utilities;

/// <summary>
/// Represents an object used to read migration sources in a particular dialect.
/// </summary>
public interface IMigrationSourceReader
{
    /// <summary>
    /// Reads header detail from the given source.
    /// </summary>
    /// <param name="reader">TextReader that encapsulates the source stream.</param>
    /// <param name="context">A context that describes the source (typically file path)</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns><see cref="SourceFileHeader"/></returns>
    Task<SourceFileHeader> ReadHeaderAsync(TextReader reader,
        string context,
        CancellationToken cancellationToken);
}