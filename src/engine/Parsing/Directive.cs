namespace Db4Wd.Parsing;

public class Directive(DirectiveType type)
{
    /// <summary>
    /// Gets the directive type.
    /// </summary>
    public DirectiveType Type => type;

    /// <summary>
    /// Defines a directive with type = <see cref="DirectiveType.None"/>
    /// </summary>
    public static readonly Directive None = new(DirectiveType.None);
    
    /// <summary>
    /// Defines a directive with type = <see cref="DirectiveType.None"/>
    /// </summary>
    public static readonly Directive Comment = new(DirectiveType.Comment);
}