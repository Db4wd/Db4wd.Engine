using DbForward.Models;

namespace DbForward.Services;

/// <summary>
/// Service that reads extension specific source files.
/// </summary>
public interface ISourceReader
{
    /// <summary>
    /// Gets the version comparer.
    /// </summary>
    /// <returns><see cref="IDbVersionComparer"/></returns>
    IDbVersionComparer GetVersionComparer();
    
    /// <summary>
    /// Creates a version id in a format that can be later parsed an ordered.
    /// </summary>
    /// <returns>Task that returns <c>string</c></returns>
    /// <remarks>
    /// This method should generate a version id that is guaranteed to be greater
    /// than any previous generated id when ordered.
    /// </remarks>
    Task<string> CreateVersionId();
    
    /// <summary>
    /// Reads the header section.
    /// </summary>
    /// <param name="textReader">Text reader that contains the source content.</param>
    /// <param name="context">A context that describes the source, typically a file path.</param>
    /// <param name="tokens">Tokens replaced in the content</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns <see cref="SourceHeader"/></returns>
    Task<SourceHeader> ReadHeaderAsync(TextReader textReader,
        string context,
        IDictionary<string, string> tokens,
        CancellationToken cancellationToken);

    /// <summary>
    /// Reads the migration section directives.
    /// </summary>
    /// <param name="textReader">Text reader that contains the source content.</param>
    /// <param name="context">A context that describes the source, typically a file path.</param>
    /// <param name="tokens">Tokens replaced in the content</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns a list of <see cref="StatementSectionDirective"/> objects.</returns>
    Task<IList<StatementSectionDirective>> ReadMigrationDirectivesAsync(TextReader textReader,
        string context,
        IDictionary<string, string> tokens,
        CancellationToken cancellationToken);

    /// <summary>
    /// Reads the rollback section directives.
    /// </summary>
    /// <param name="textReader">Text reader that contains the source content.</param>
    /// <param name="context">A context that describes the source, typically a file path.</param>
    /// <param name="tokens">Tokens replaced in the content</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns a list of <see cref="StatementSectionDirective"/> objects.</returns>
    Task<IList<StatementSectionDirective>> ReadRollbackDirectivesAsync(TextReader textReader,
        string context,
        IDictionary<string, string> tokens,
        CancellationToken cancellationToken);
}