namespace Db4Wd.Parsing;

public sealed class StatementDirective(string value) : Directive(DirectiveType.Statement)
{
    public string Value => value;
}