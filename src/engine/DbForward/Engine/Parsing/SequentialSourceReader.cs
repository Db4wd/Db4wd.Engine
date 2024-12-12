using System.Collections.Immutable;

namespace DbForward.Engine.Parsing;

/// <summary>
/// Represents a <see cref="ISourceReader"/> that parses sequentially structured source content.
/// </summary>
public abstract class SequentialSourceReader(
    IReadOnlyDictionary<string, string> tokenDictionary) 
    : ISourceReader
{
    protected IReadOnlyDictionary<string, string> TokenDictionary { get; } = tokenDictionary;
    
    /// <inheritdoc />
    public async Task<SourceHeader> ReadHeaderAsync(TextReader textReader, 
        CancellationToken cancellationToken)
    {
        using var cancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        
        var migrationId = default(string);
        var metadata = new Dictionary<string, string>();

        await ReadSectionAsync(textReader,
            GetHeaderSectionTag,
            context =>
            {
                if (TryParseMigrationId(context, out var id))
                {
                    if (id == null)
                    {
                        migrationId = id;
                        return;
                    }

                    throw new ApplicationException($"line {context.LineNumber}: duplicate migration id directive");
                }
                
                if (!TryParseMetadataTag(context, out var tag))
                    return;

                if (metadata.TryAdd(tag.Key, tag.Value))
                    return;

                throw new ApplicationException($"line {context.LineNumber}: duplicate metadata tag '{tag.Key}'");
            },
            cancellationToken);

        return new SourceHeader(
            migrationId ?? throw new ApplicationException("missing migration id directive"),
            metadata);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StatementDirective>> ReadMigrationDirectivesAsync(
        TextReader textReader, 
        CancellationToken cancellationToken)
    {
        var directives = new List<StatementDirective>(32);
        
        await ReadSectionAsync(textReader,
            GetMigrationSectionTag,
            context => directives.Add(ParseStatementDirective(context)),
            cancellationToken);

        return directives;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StatementDirective>> ReadRollbackDirectivesAsync(
        TextReader textReader, 
        CancellationToken cancellationToken)
    {
        var directives = new List<StatementDirective>(32);
        
        await ReadSectionAsync(textReader,
            GetRollbackSectionTag,
            context => directives.Add(ParseStatementDirective(context)),
            cancellationToken);

        return directives;
    }

    protected abstract StatementDirective ParseStatementDirective(ParseContext context);

    protected abstract bool TryParseMetadataTag(ParseContext context, out KeyValuePair<string, string> tag);

    protected abstract bool TryParseMigrationId(ParseContext context, out string? id);

    protected abstract SectionTag GetHeaderSectionTag(string line);

    protected abstract SectionTag GetMigrationSectionTag(string line);

    protected abstract SectionTag GetRollbackSectionTag(string line);

    protected virtual string TransformLineContent(string line) => TokenDictionary
        .Aggregate(line, (current, next) => current.Replace(next.Key, next.Value));

    private async Task ReadSectionAsync(TextReader textReader,
        Func<string, SectionTag> tagSelector,
        Action<ParseContext> callback,
        CancellationToken cancellationToken)
    {
        var lineNumber = 0;
        var scanning = false;
        
        while (await textReader.ReadLineAsync(cancellationToken) is { } str && ++lineNumber > 0)
        {
            if (string.IsNullOrWhiteSpace(str))
                continue;

            switch (scanning, tag: tagSelector(str))
            {
                case { scanning: false, tag: SectionTag.StartTag }:
                    scanning = true;
                    continue;
                
                case { scanning: true, tag: SectionTag.EndTag }:
                    return;
                
                case { scanning: true }:
                    var postValue = TransformLineContent(str);
                    callback(new ParseContext(postValue, lineNumber));
                    break;
            }
        }
    }
}