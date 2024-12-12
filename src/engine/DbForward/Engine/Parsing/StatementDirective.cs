namespace DbForward.Engine.Parsing;

public abstract class StatementDirective(ParseContext context)
{
    public int LineNumber => context.LineNumber;

    public string? Value => context.LineValue;

    /// <inheritdoc />
    public override string ToString() => $"{context.LineNumber:D4} {context.LineValue ?? "(empty)"}";
}

public sealed class PartialStatementDirective(ParseContext context) : StatementDirective(context)
{
}

public sealed class TerminatedStatementDirective(ParseContext context) : StatementDirective(context)
{
}

public sealed class LogControlDirective(ParseContext context, bool enable) : StatementDirective(context)
{
    public bool EnableLogging { get; } = enable;
}

public sealed class IgnoredStatementDirective(ParseContext context) : StatementDirective(context)
{
}

public sealed class PragmaControlDirective(ParseContext context, string pragma, bool enabled) 
    : StatementDirective(context)
{
    public string Pragma { get; } = pragma;
    public bool Enabled { get; } = enabled;
}