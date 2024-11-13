namespace DbForward.Models;

public record SourceHeader(
    string Context,
    Guid MigrationId,
    string DbVersion,
    IDictionary<string, string> Metadata)
{
    /// <inheritdoc />
    public override string ToString() => Context;
}