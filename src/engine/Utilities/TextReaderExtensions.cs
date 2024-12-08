using System.Collections.ObjectModel;
using System.Text;

namespace DbForward.Utilities;

public static class TextReaderExtensions
{
    public static async Task<string> ReadAllTextAsync(this TextReader textReader,
        IReadOnlyDictionary<string, string>? tokens,
        CancellationToken cancellationToken = default)
    {
        var buffer = new StringBuilder(4096);
        tokens ??= ReadOnlyDictionary<string, string>.Empty;
        
        while (await textReader.ReadLineAsync(cancellationToken) is { } str)
        {
            buffer.AppendLine(tokens.Aggregate(str, (next, kv) => next.Replace(kv.Key, kv.Value)));
        }

        return buffer.ToString();
    }
}