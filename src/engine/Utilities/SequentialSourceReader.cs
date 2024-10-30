using Db4Wd.Parsing;

namespace Db4Wd.Utilities;

public abstract class SequentialSourceReader : IMigrationSourceReader
{
    /// <inheritdoc />
    public async Task<SourceFileHeader> ReadHeaderAsync(
        TextReader reader, 
        string context, 
        CancellationToken cancellationToken)
    {
        const int beforeHeader = 0;
        const int inHeader = 1;
        var state = beforeHeader;
        var lineNumber = 0;
        MigrationIdDirective? idDirective = null;
        DbVersionDirective? versionDirective = null;
        var metadata = new Dictionary<string, string>();
        
        while (await reader.ReadLineAsync(cancellationToken) is { } str)
        {
            lineNumber++;
            
            if (string.IsNullOrWhiteSpace(str))
                continue;

            var directive = MatchHeaderDirective(str);

            switch (directive, state)
            {
                case { directive.Type: DirectiveType.StartHeader, state: beforeHeader }:
                    state = inHeader;
                    continue;
                
                case { directive.Type: DirectiveType.StartHeader }:
                    throw new MigrationSourceException("Invalid start header directive", context, lineNumber);
                
                case { directive.Type: DirectiveType.EndHeader, state: inHeader }:
                    break;
                
                case { directive.Type: DirectiveType.EndHeader }:
                    throw new MigrationSourceException("Invalid end header directive", context, lineNumber);
                
                case { directive: MigrationIdDirective idMatch, state: inHeader } when idDirective == null:
                    idDirective = idMatch;
                    continue;
                
                case { directive: DbVersionDirective versionMatch, state: inHeader } when versionDirective == null:
                    versionDirective = versionMatch;
                    continue;
                
                case { directive: MetadataDirective metadataMatch, state: inHeader }:
                    metadata[metadataMatch.Key] = metadata[metadataMatch.Value];
                    continue;
                
                case { directive.Type: DirectiveType.None }:
                case { directive.Type: DirectiveType.Comment }:
                    continue;
                
                case { state: inHeader }:
                    throw new MigrationSourceException($"Invalid directive or content in header: '{str}'",
                        context,
                        lineNumber);
                    
                default:
                    continue;
            }
        }

        if (idDirective == null)
        {
            throw new MigrationSourceException("Source file missing migration id directive", context);
        }

        if (versionDirective == null)
        {
            throw new MigrationSourceException("Source file missing db version directive", context);
        }

        return new SourceFileHeader(context, idDirective.Value, versionDirective.Value, metadata.ToArray());
    }

    protected abstract Directive MatchHeaderDirective(string str);
}