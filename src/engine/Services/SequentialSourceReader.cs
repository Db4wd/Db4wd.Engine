using System.Text;
using DbForward.Constants;
using DbForward.Logging;
using DbForward.Models;
using Microsoft.Extensions.Logging;

namespace DbForward.Services;

public abstract class SequentialSourceReader : ISourceReader
{
    public abstract class LineContext
    {
        public abstract int LineNumber { get; }
        
        public abstract string LineText { get; }
        
        public abstract string Context { get; }
        
        public abstract SequentialSection? Section { get; }

        public LoggerCallbackException GetLineNumberException(string message)
        {
            return new LoggerCallbackException(log => log.LogError(
                "Source {source}/line {line}: {message}",
                Context,
                LineNumber,
                message));
        }

        public LoggerCallbackException GetSourceException(string message)
        {
            return new LoggerCallbackException(log => log.LogError(
                "Source {source}: {message}",
                Context,
                message));
        }
    }
    
    private sealed class PrivateLineContext(string context) : LineContext
    {
        private int lineNumber;
        private string lineText = string.Empty;
        private SequentialSection? section;

        /// <inheritdoc />
        public override int LineNumber => lineNumber;

        /// <inheritdoc />
        public override string LineText => lineText;

        /// <inheritdoc />
        public override string Context => context;

        /// <inheritdoc />
        public override SequentialSection? Section => section;

        public bool Next(string text)
        {
            ++lineNumber;
            lineText = text;
            return true;
        }

        public void SetSection(SequentialSection? value) => section = value;
    }
    
    /// <inheritdoc />
    public async Task<SourceHeader> ReadHeaderAsync(TextReader textReader,
        string context,
        IDictionary<string, string> tokens,
        CancellationToken cancellationToken)
    {
        Guid? migrationId = null;
        string? dbVersion = null;
        var metadata = new Dictionary<string, string>();

        await ReadSectionSyntaxAsync(
            textReader,
            context,
            SequentialSection.Header,
            tokens,
            lineContext =>
            {
                switch (ReadHeaderTagSyntax(lineContext))
                {
                    case MigrationIdTagSyntax syntax when migrationId is null:
                        migrationId = syntax.Value;
                        break;
                    
                    case MigrationIdTagSyntax:
                        throw lineContext.GetLineNumberException("Duplicate migration id tag");
                    
                    case DbVersionTagSyntax syntax when dbVersion is null:
                        dbVersion = syntax.Value;
                        break;
                    
                    case DbVersionTagSyntax:
                        throw lineContext.GetLineNumberException("Duplicate dbVersion tag");
                    
                    case MetadataTagSyntax syntax when metadata.TryAdd(syntax.Value.Key, syntax.Value.Value):
                        break;
                    
                    case MetadataTagSyntax syntax:
                        throw lineContext.GetLineNumberException($"Duplicate metadata key '{syntax.Value.Key}'");
                }
            },
            cancellationToken);

        return ComposeHeaderResult(context, migrationId, dbVersion, metadata);
    }

    /// <inheritdoc />
    public async Task<IList<StatementSectionDirective>> ReadMigrationDirectivesAsync(TextReader textReader,
        string context,
        IDictionary<string, string> tokens,
        CancellationToken cancellationToken)
    {
        return await ReadSectionDirectivesAsync(textReader,
            context,
            tokens,
            SequentialSection.MigrationStatements,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IList<StatementSectionDirective>> ReadRollbackDirectivesAsync(TextReader textReader,
        string context,
        IDictionary<string, string> tokens,
        CancellationToken cancellationToken)
    {
        return await ReadSectionDirectivesAsync(textReader,
            context,
            tokens,
            SequentialSection.RollbackStatements,
            cancellationToken);
    }

    protected abstract SectionTagSyntax? ReadSectionTagSyntax(LineContext context);

    protected abstract HeaderTagSyntax? ReadHeaderTagSyntax(LineContext context);

    protected abstract StatementSectionSyntax? ReadStatementSyntax(LineContext context);

    private async Task<IList<StatementSectionDirective>> ReadSectionDirectivesAsync(TextReader textReader,
        string context,
        IDictionary<string, string> tokens,
        SequentialSection section,
        CancellationToken cancellationToken)
    {
        var directives = new List<StatementSectionDirective>(512);
        var buffer = new StringBuilder();

        await ReadSectionSyntaxAsync(
            textReader,
            context,
            section,
            tokens,
            lineContext =>
            {
                switch (ReadStatementSyntax(lineContext))
                {
                    case TerminatedStatementSyntax syntax:
                        TryAddBufferLine(syntax.Content);
                        FlushDirective();
                        break;
                    
                    case PartialStatementSyntax syntax:
                        TryAddBufferLine(syntax.Content);
                        break;
                    
                    case DirectiveSyntax syntax:
                        FlushDirective();
                        directives.Add(syntax.Directive);
                        break;
                }
            }, cancellationToken);
        
        FlushDirective();
        return directives;

        void TryAddBufferLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;
            buffer.AppendLine(line);
        }
        
        void FlushDirective()
        {
            if (buffer.Length == 0)
                return;
            
            directives.Add(new TerminatedStatementDirective(buffer.ToString()));
            buffer.Clear();
        }
    }

    private async Task ReadSectionSyntaxAsync(TextReader textReader,
        string context,
        SequentialSection section,
        IDictionary<string, string> tokens,
        Action<LineContext> callback,
        CancellationToken cancellationToken)
    {
        var lineContext = new PrivateLineContext(context);

        while (await textReader.ReadLineAsync(cancellationToken) is { } content)
        {
            content = tokens
                .Aggregate(content, (next, kv) => next.Replace(kv.Key, kv.Value))
                .TrimEnd();

            lineContext.Next(content);
            
            if (string.IsNullOrWhiteSpace(content))
                continue;

            switch (ReadSectionTagSyntax(lineContext))
            {
                case SectionOpenTagSyntax syntax when lineContext.Section is null:
                    lineContext.SetSection(syntax.Section);
                    continue;
                
                case SectionCloseTagSyntax syntax when syntax.Section == lineContext.Section:
                    lineContext.SetSection(null);
                    continue;
                
                case SectionOpenTagSyntax:
                case SectionCloseTagSyntax:
                    throw lineContext.GetLineNumberException("Invalid section tag");
            }

            if (lineContext.Section != section)
                continue;

            callback(lineContext);
        }
    }

    protected static LoggerCallbackException CreateLineNumberException(
        string context,
        int lineNumber,
        string lineContent,
        string message)
    {
        return new LoggerCallbackException(log => log.LogError(
            """
            Source {source}/line {lineNubber}: {message}\n
                  {lineContent}      
            """,
            context,
            lineNumber,
            message,
            new VerboseToken(lineContent)));
    }

    private static SourceHeader ComposeHeaderResult(string context, 
        Guid? migrationId, 
        string? dbVersion,
        Dictionary<string, string> metadata)
    {
        return (migrationId, dbVersion) switch
        {
            { migrationId: not null, dbVersion: not null } => new SourceHeader(
                context,
                migrationId.Value,
                dbVersion,
                metadata),
            { migrationId: null } => throw new LoggerCallbackException(e => e.LogError("Source {source}: missing migration id tag",
                    context)),
            { dbVersion: null } => throw new LoggerCallbackException(e => e.LogError("Source {source}: missing dbVersion tag",
                context))
        };
    }
}