using Db4Wd.Parsing;
using Db4Wd.Utilities;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Services;

public sealed class CachingSourceFileLoader(ILogger<CachingSourceFileLoader> logger) : ISourceFileLoader
{
    private IReadOnlyCollection<SourceFileHeader>? _cachedResult;
    
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<SourceFileHeader>> GetSourceFilesAsync(
        IMigrationSourceReader sourceReader,
        DirectoryInfo basePath,
        string matchPattern,
        CancellationToken cancellationToken)
    {
        if (_cachedResult != null)
        {
            logger.LogDebug("Using cached {item} collection from previous call (count={count})", 
                nameof(SourceFileHeader),
                _cachedResult.Count);
            return _cachedResult;
        }

        return _cachedResult = await LoadSourceFilesAsync(sourceReader, basePath, matchPattern, cancellationToken);
    }

    private async Task<IReadOnlyCollection<SourceFileHeader>> LoadSourceFilesAsync(
        IMigrationSourceReader sourceReader, 
        DirectoryInfo basePath, 
        string matchPattern, 
        CancellationToken cancellationToken)
    {
        var matcher = new Matcher().AddInclude(matchPattern);
        
        logger.LogInformation("Scanning {path} for source files using pattern {pattern}",
            basePath,
            matchPattern);

        var matchedPaths = matcher.GetResultsInFullPath(basePath.FullName);

        return await Task.WhenAll(matchedPaths.Select(async path => await LoadSourceFileAsync(
            path, 
            sourceReader, 
            cancellationToken)));
    }

    private async Task<SourceFileHeader> LoadSourceFileAsync(
        string path, 
        IMigrationSourceReader sourceReader, 
        CancellationToken cancellationToken)
    {
        using var textReader = new StreamReader(File.OpenRead(path));
        var header = await sourceReader.ReadHeaderAsync(textReader, path, cancellationToken);

        return header switch
        {
            { MigrationId: null } => throw new MigrationSourceException("Source file missing migration id directive", path),
            { DbVersion: null } => throw new MigrationSourceException("Source file missing database version directive", path),
            _ => LogValid(header)
        };
    }

    private SourceFileHeader LogValid(SourceFileHeader header)
    {
        logger.LogDebug("Validated {path} (migration id={id}, dbVersion={version})",
            header.Context,
            header.MigrationId,
            header.DbVersion);

        return header;
    }
}