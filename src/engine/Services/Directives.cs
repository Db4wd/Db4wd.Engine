namespace DbForward.Services;

public abstract class StatementSectionDirective(string text)
{
    /// <summary>
    /// Gets the directive text.
    /// </summary>
    public string Text => text;

    /// <inheritdoc />
    public override string ToString() => Text;
}

public sealed class PartialStatementDirective(string text) : StatementSectionDirective(text)
{
}

public sealed class TerminatedStatementDirective(string text = "") : StatementSectionDirective(text)
{
}