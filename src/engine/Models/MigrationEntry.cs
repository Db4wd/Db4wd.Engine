namespace Db4Wd.Models;

public record MigrationEntry(
    Guid Id,
    int DbVersion,
    DateTime DateApplied,
    string Path,
    string Filename,
    string Sha);