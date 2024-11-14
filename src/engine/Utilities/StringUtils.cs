namespace DbForward.Utilities;

internal static class StringUtils
{
    internal static string TruncateEnd(
        this string str,
        int length,
        string prefix = "...")
    {
        return str.Length <= length
            ? str
            : $"{prefix}{str[^length..]}";
    }
}