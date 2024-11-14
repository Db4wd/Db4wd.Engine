using System.Collections.Concurrent;
using DbForward.Services;
using Microsoft.Extensions.FileSystemGlobbing;

namespace UnitTests;

public sealed class ConcurrentFileSystem : AbstractFileSystem
{
    private static readonly ConcurrentDictionary<string, byte[]> StreamData = new();
    
    /// <inheritdoc />
    public override IEnumerable<string> MatchPaths(string basePath, string matchPattern) =>
        new Matcher().AddInclude(matchPattern).GetResultsInFullPath(basePath);

    /// <inheritdoc />
    public override void CreateDirectory(string path)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override Stream GetStream(string path, FileMode fileMode)
    {
        var bytes = StreamData.GetOrAdd(path, File.ReadAllBytes);
        return new MemoryStream(bytes);
    }
}