namespace Db4Wd.Parsing;

public enum DirectiveType
{
    None,
    Comment,
    StartHeader,
    EndHeader,
    MigrationId,
    DbVersion,
    Metadata,
    StartTransaction,
    CommitTransaction,
    CommitBatch,
    Statement,
    MigrationStart,
    MigrationEnd,
    RollbackStart,
    RollbackEnd,
    Extension
}