using DbForward.Models;

namespace DbForward.Services;

public interface ISourceFileManager
{
    /// <summary>
    /// Provides source headers matched in a path.
    /// </summary>
    /// <param name="basePath">Base path to search.</param>
    /// <param name="searchPattern">Pattern used to identify source files</param>
    /// <param name="sourceReader">Reader capable of parsing headers</param>
    /// <param name="tokens">Tokens to replace in the source file</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns a collection of <see cref="SourceHeader"/></returns>
    Task<IList<SourceHeader>> GetSourceHeadersInPathAsync(
        DirectoryInfo basePath,
        string searchPattern,
        ISourceReader sourceReader,
        IDictionary<string, string> tokens,
        CancellationToken cancellationToken);
}

public sealed class SourceFileManager(IFileSystem fileSystem) : ISourceFileManager
{
    /// <inheritdoc />
    public async Task<IList<SourceHeader>> GetSourceHeadersInPathAsync(
        DirectoryInfo basePath, 
        string searchPattern,
        ISourceReader sourceReader,
        IDictionary<string, string> tokens,
        CancellationToken cancellationToken)
    {
        var matchedPaths = fileSystem.MatchPaths(basePath.FullName, searchPattern);

        return await Task.WhenAll(matchedPaths.Select(async path =>
        {
            using var textReader = fileSystem.CreateReader(path);
            return await sourceReader.ReadHeaderAsync(
                textReader, 
                Path.GetFullPath(path), 
                tokens, 
                cancellationToken);
        }));
    }
}