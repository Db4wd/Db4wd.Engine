using System.Text.RegularExpressions;
using DbForward.Engine.Parsing;
using DbForward.Engine.Services;

namespace DbForward.Postgres;

internal sealed class PostgresSourceReader(IReadOnlyDictionary<string, string> tokenDictionary) 
    : SequentialSourceReader(tokenDictionary)
{
    private sealed class FactoryImpl : IParameterizedFactory<IReadOnlyDictionary<string, string>, ISourceReader>
    {
        /// <inheritdoc />
        public ISourceReader Create(IReadOnlyDictionary<string, string> tokenDictionary)
        {
            return new PostgresSourceReader(tokenDictionary);
        }
    }

    internal static IParameterizedFactory<IReadOnlyDictionary<string, string>, ISourceReader> Factory { get; }
        = new FactoryImpl();
    
    /// <inheritdoc />
    protected override StatementDirective ParseStatementDirective(ParseContext context)
    {
        if (context.LineValue.EndsWith(';'))
        {
            return new TerminatedStatementDirective(context);
        }

        if (Regex.Match(context.LineValue, @"^-- \[ifdef (.*?)]$") is { Success: true } ifMatch)
        {
            var ifKey = ifMatch.Groups[1].Value;
            return TokenDictionary.ContainsKey(ifKey)
                ? new PragmaControlDirective(context, ifKey, true)
                : new IgnoredStatementDirective(context);
        }
        
        if (Regex.Match(context.LineValue, @"^-- \[ifndef (.*?)]$") is { Success: true } ifnMatch)
        {
            var ifnKey = ifnMatch.Groups[1].Value;
            return !TokenDictionary.ContainsKey(ifnKey)
                ? new PragmaControlDirective(context, ifnKey, true)
                : new IgnoredStatementDirective(context);
        }
        
        if (Regex.Match(context.LineValue, @"^-- \[endif (.*?)]$") is { Success: true } endIfMatch)
        {
            var endIfKey = endIfMatch.Groups[1].Value;
            return new PragmaControlDirective(context, endIfKey, false);
        }

        if (context.LineValue.StartsWith("--"))
        {
            return new IgnoredStatementDirective(context);
        }

        return new PartialStatementDirective(context);
    }

    /// <inheritdoc />
    protected override bool TryParseMetadataTag(ParseContext context, out KeyValuePair<string, string> tag)
    {
        tag = default;

        if (!context.LineValue.StartsWith("-- [metadata."))
            return false;

        if (Regex.Match(context.LineValue, @"^-- \[metadata.(.*?): (.*?)]$") is not { Success: true } match)
            return false;

        tag = new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value);
        return true;
    }

    /// <inheritdoc />
    protected override bool TryParseMigrationId(ParseContext context, out string? id)
    {
        id = default;

        if (!context.LineValue.StartsWith("-- [id:"))
            return false;

        if (MigrationId.TryParse(context.LineValue, out id))
            return true;

        throw new ApplicationException($"line {context.LineNumber}: invalid migration id");
    }

    /// <inheritdoc />
    protected override SectionTag GetHeaderSectionTag(string line) => line switch
    {
        "-- [head]" => SectionTag.StartTag,
        "-- [/head]" => SectionTag.EndTag,
        _ => SectionTag.None
    };

    /// <inheritdoc />
    protected override SectionTag GetMigrationSectionTag(string line) => line switch
    {
        "-- [up]" => SectionTag.StartTag,
        "-- [/up]" => SectionTag.EndTag,
        _ => SectionTag.None
    };

    /// <inheritdoc />
    protected override SectionTag GetRollbackSectionTag(string line) => line switch
    {
        "-- [down]" => SectionTag.StartTag,
        "-- [/down]" => SectionTag.EndTag,
        _ => SectionTag.None
    };
}