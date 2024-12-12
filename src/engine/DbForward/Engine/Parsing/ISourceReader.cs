namespace DbForward.Engine.Parsing;

public interface ISourceReader
{
    Task<SourceHeader> ReadHeaderAsync(
        TextReader textReader,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StatementDirective>> ReadMigrationDirectivesAsync(
        TextReader textReader,
        CancellationToken cancellationToken);
    
    Task<IReadOnlyList<StatementDirective>> ReadRollbackDirectivesAsync(
        TextReader textReader,
        CancellationToken cancellationToken);
}