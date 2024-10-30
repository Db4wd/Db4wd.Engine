namespace Db4Wd.Parsing;

public sealed class DbVersionDirective(int value) : Directive(DirectiveType.DbVersion)
{
    public int Value => value;
}