namespace Db4Wd.Parsing;

/// <summary>
/// Defines the metadata of a source file.
/// </summary>
/// <param name="Context">Context that describes the source</param>
/// <param name="MigrationId">Migration id</param>
/// <param name="DbVersion">Database version</param>
/// <param name="Metadata">Metadata key/value pairs</param>
public record SourceFileHeader(
    string Context,
    Guid MigrationId,
    int DbVersion,
    KeyValuePair<string, string>[] Metadata)
{
}