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

        if (DirectiveStandardConventions.TryGetMigrationId(directivePart, out var id))
            return new MigrationIdDirective(id);

        if (DirectiveStandardConventions.TryGetDbVersionId(directivePart, out var version))
            return new DbVersionDirective(version);

        if (DirectiveStandardConventions.TryGetMetadataPair(directivePart, out var metadata))
            return new MetadataDirective(metadata.Key, metadata.Value);

        return new CommentDirective(directivePart);
    }
}