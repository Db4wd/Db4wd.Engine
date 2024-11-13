using DbForward.Constants;

namespace DbForward.Services;

/// <summary>
/// Base type for syntax found by <see cref="SequentialSourceReader"/>
/// </summary>
public abstract class SequentialSourceSyntax
{
}

/// <summary>
/// Defines syntax that is ignored.
/// </summary>
public sealed class IgnoredSourceSyntax : SequentialSourceSyntax
{
    private IgnoredSourceSyntax()
    {
    }
    
    public static readonly IgnoredSourceSyntax Instance = new();

    /// <inheritdoc />
    public override string ToString() => "(ignored)";
}

/// <summary>
/// Defines syntax that is invalid.
/// </summary>
public sealed class InvalidSyntax(string? message = null) : SequentialSourceSyntax
{
    public string? Message { get; } = message;

    public static readonly InvalidSyntax Instance = new();
    
    public override string ToString() => "(invalid)";
}

/// <summary>
/// Base type for section syntax types.
/// </summary>
/// <param name="section"></param>
public abstract class SectionTagSyntax(SequentialSection section) : SequentialSourceSyntax
{
    public SequentialSection Section { get; } = section;

    /// <inheritdoc />
    public override string ToString() => $"{Section}";
}

/// <summary>
/// Defines a section's open tag syntax.
/// </summary>
/// <param name="section"></param>
public sealed class SectionOpenTagSyntax(SequentialSection section) :
    SectionTagSyntax(section)
{
}

/// <summary>
/// Defines a section's close tag syntax.
/// </summary>
/// <param name="section"></param>
public sealed class SectionCloseTagSyntax(SequentialSection section) :
    SectionTagSyntax(section)
{
}

/// <summary>
/// Base type for header tag syntax.
/// </summary>
public abstract class HeaderTagSyntax : SequentialSourceSyntax
{
}

/// <summary>
/// Base type for syntax found in the header section.
/// </summary>
/// <param name="value">Tag value</param>
/// <typeparam name="T">Tag value type</typeparam>
public abstract class HeaderTagSyntax<T>(T value) : HeaderTagSyntax
{
    public T Value { get; } = value;

    /// <inheritdoc />
    public override string ToString() => $"{Value}";
}

/// <summary>
/// Defines a migration id tag.
/// </summary>
/// <param name="value">Migration id</param>
public sealed class MigrationIdTagSyntax(Guid value) : HeaderTagSyntax<Guid>(value)
{
}

/// <summary>
/// Defines a version tag.
/// </summary>
/// <param name="value">Version tag value</param>
public sealed class DbVersionTagSyntax(string value) : HeaderTagSyntax<string>(value)
{
}

/// <summary>
/// Defines a metadata tag
/// </summary>
/// <param name="key">Metadata key</param>
/// <param name="value">metadata value</param>
public sealed class MetadataTagSyntax(string key, string value) : HeaderTagSyntax<KeyValuePair<string, string>>(
    new KeyValuePair<string, string>(key, value))
{
}

/// <summary>
/// Base type for statement syntax
/// </summary>
/// <param name="content">Statement content</param>
public abstract class StatementSectionSyntax(string content) : SequentialSourceSyntax
{
    public string Content { get; } = content;

    /// <inheritdoc />
    public override string ToString() => Content;
}

/// <summary>
/// Defines a partial statement.
/// </summary>
/// <param name="content">Statement content</param>
public sealed class PartialStatementSyntax(string content) : StatementSectionSyntax(content)
{
}

/// <summary>
/// Defines a terminated statement.
/// </summary>
/// <param name="content">Statement content.</param>
public sealed class TerminatedStatementSyntax(string content = "") : StatementSectionSyntax(content)
{
}

/// <summary>
/// Defines an extension directive syntax.
/// </summary>
/// <param name="directive">The directive.</param>
public sealed class DirectiveSyntax(StatementSectionDirective directive) : StatementSectionSyntax(string.Empty)
{
    public StatementSectionDirective Directive { get; } = directive;

    /// <inheritdoc />
    public override string ToString() => $"{Directive}";
}