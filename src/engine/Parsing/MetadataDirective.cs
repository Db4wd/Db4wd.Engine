namespace Db4Wd.Parsing;

public sealed class MetadataDirective(string key, string value) : Directive(DirectiveType.Metadata)
{
    public string Key => key;

    public string Value => value;
}