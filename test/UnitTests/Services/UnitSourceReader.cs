using System.Text.RegularExpressions;
using DbForward.Constants;
using DbForward.Services;

namespace UnitTests.Services;

internal sealed class UnitSourceReader : ConventionalSourceReader
{
    internal new static readonly UnitSourceReader Instance = new();

    /// <inheritdoc />
    protected override StatementSectionSyntax? ReadStatementSyntax(LineContext context)
    {
        var text = context.LineText;
        
        return text switch
        {
            "-- [custom]" => new DirectiveSyntax(new CustomDirective()),
            _ => base.ReadStatementSyntax(context)
        };
    }
}