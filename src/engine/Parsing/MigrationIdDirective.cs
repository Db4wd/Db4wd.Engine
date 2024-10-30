namespace Db4Wd.Parsing;

public sealed class MigrationIdDirective(Guid value) : Directive(DirectiveType.MigrationId)
{
    public Guid Value => value;
}