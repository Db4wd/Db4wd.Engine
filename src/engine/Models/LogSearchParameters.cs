namespace DbForward.Models;

public record LogSearchParameters(
    int Limit,
    Guid? MigrationId,
    DateTime? RangeStart,
    DateTime? RangeEnd,
    string? Operation,
    string? Agent,
    string? Host,
    string? DbVersion,
    string? MetadataKey,
    string? MetadataValue,
    string? SourcePath,
    string? SourceFile,
    string? Sha);