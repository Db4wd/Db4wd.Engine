using System.Text.RegularExpressions;
using DbForward.Constants;

namespace DbForward.Services;

public partial class ConventionalSourceReader : SequentialSourceReader
{
    public static ISourceReader Instance { get; } = new ConventionalSourceReader();
    
    [GeneratedRegex(@"^-- \[metadata.(?<key>[^:]+): (?<value>.+)\]")]
    private static partial Regex KeyValuePairRegex();

    [GeneratedRegex(@"^-- \[dbVersion: (?<value>[\d]{4}-[\d]{2}-[\d]{2}T[\d]{2}:[\d]{2}:[\d]{2})\]")]
    private static partial Regex DbVersionRegex();

    [GeneratedRegex(@"^-- \[id: (?<id>[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\]")]
    private static partial Regex MigrationIdRegex();

    /// <inheritdoc />
    protected override SectionTagSyntax? ReadSectionTagSyntax(LineContext context)
    {
        return context.LineText switch
        {
            "-- [head]" => new SectionOpenTagSyntax(SequentialSection.Header),
            "-- [/head]" => new SectionCloseTagSyntax(SequentialSection.Header),
            "-- [up]" => new SectionOpenTagSyntax(SequentialSection.MigrationStatements),
            "-- [/up]" => new SectionCloseTagSyntax(SequentialSection.MigrationStatements),
            "-- [down]" => new SectionOpenTagSyntax(SequentialSection.RollbackStatements),
            "-- [/down]" => new SectionCloseTagSyntax(SequentialSection.RollbackStatements),
            _ => null
        };
    }

    /// <inheritdoc />
    protected override HeaderTagSyntax? ReadHeaderTagSyntax(LineContext context)
    {
        var text = context.LineText;
        
        if (MigrationIdRegex().Match(text) is { Success: true } id)
            return new MigrationIdTagSyntax(Guid.Parse(id.Groups["id"].Value));

        if (DbVersionRegex().Match(text) is { Success: true } version)
            return new DbVersionTagSyntax(version.Groups["value"].Value);

        if (KeyValuePairRegex().Match(text) is { Success: true } kv)
            return new MetadataTagSyntax(kv.Groups["key"].Value, kv.Groups["value"].Value); 
        
        if (text.StartsWith("--"))
            return null;

        throw context.GetLineNumberException("Unexpected token");
    }

    /// <inheritdoc />
    protected override StatementSectionSyntax? ReadStatementSyntax(LineContext context)
    {
        var text = context.LineText;
        
        return text switch
        {
            "-- [go]" => new TerminatedStatementSyntax(),
            not null when text.StartsWith("--") => null,
            _ => new PartialStatementSyntax(text!)
        };
    }
}