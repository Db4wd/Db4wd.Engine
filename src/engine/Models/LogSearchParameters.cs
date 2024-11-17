namespace DbForward.Models;

public record LogSearchParameters(
    int Limit,
    Guid? MigrationId,
    DateTime? RangeStart,
    DateTime? RangeEnd,
    string? Operation,
    string? Agent,
    string? Host);