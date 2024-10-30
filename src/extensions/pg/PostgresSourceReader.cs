using Db4Wd.Parsing;
using Db4Wd.Utilities;

namespace Db4Wd.Postgres;

public sealed class PostgresSourceReader : SequentialSourceReader
{
    /// <inheritdoc />
    protected override Directive MatchHeaderDirective(string str)
    {
        if (!str.StartsWith("-- "))
        {
            return new StatementDirective(str);
        }

        var directivePart = str[3..];

        return directivePart switch
        {
            "{head}" => new Directive(DirectiveType.StartHeader),
            "{~head}" => new Directive(DirectiveType.EndHeader),
            not null when DirectiveStandardConventions.TryGetMigrationId(directivePart, out var id) =>
                new MigrationIdDirective(id),
            not null when DirectiveStandardConventions.TryGetDbVersionId(directivePart, out var version) =>
                new DbVersionDirective(version),
            not null when DirectiveStandardConventions.TryGetMetadataPair(directivePart, out var metadata) =>
                new MetadataDirective(metadata.Key, metadata.Value),
            not null => new CommentDirective(directivePart),
            _ => Directive.None
        };
    }
}