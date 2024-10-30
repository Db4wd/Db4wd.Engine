namespace Db4Wd.Parsing;

public sealed class CommentDirective(string value) : Directive(DirectiveType.Comment)
{
    public string Value => value;
}