using Microsoft.Extensions.FileSystemGlobbing;

namespace DbForward.Services;

internal sealed class PhysicalFileSystem : AbstractFileSystem
{
    /// <inheritdoc />
    public override IEnumerable<string> MatchPaths(string basePath, string matchPattern) =>
        new Matcher().AddInclude(matchPattern).GetResultsInFullPath(basePath);

    /// <inheritdoc />
    public override void CreateDirectory(string path) => Directory.CreateDirectory(path);

    /// <inheritdoc />
    protected override Stream GetStream(string path, FileMode fileMode)
    {
        return File.Open(path, fileMode);
    }
}