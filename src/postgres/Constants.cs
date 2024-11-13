using System.Reflection;

namespace DbForward.Postgres;

internal static class Constants
{
    internal const string ToolName = "pgfwd";
    internal const string SchemaName = "pgfwd_meta";

    internal static readonly string RootAssetPath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
}