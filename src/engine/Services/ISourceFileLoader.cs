using Db4Wd.Parsing;
using Db4Wd.Utilities;

namespace Db4Wd.Services;

public interface ISourceFileLoader
{
    /// <summary>
    /// Gets source files from a base path using a match pattern.
    /// </summary>
    /// <param name="sourceReader">Object that parses source files</param>
    /// <param name="basePath">Base path where files are location</param>
    /// <param name="matchPattern">Match pattern</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Collection of <see cref="SourceFileHeader"/></returns>
    Task<SourceFileHeader[]> GetSourceFilesAsync(
        IMigrationSourceReader sourceReader,
        DirectoryInfo basePath,
        string matchPattern,
        CancellationToken cancellationToken);
}