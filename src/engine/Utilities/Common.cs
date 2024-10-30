using Db4Wd.Extensions;

namespace Db4Wd.Utilities;

public static class Common
{
    private const string EnvironmentFileSuffix = "ENV_FILE";
    public static string ProgramDataPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".db4wd");

    public static string EnvDataPath { get; } = Path.Combine(ProgramDataPath, "env");

    public static bool IsEnvironmentFilePathVariable(string rootContext, string key) =>
        key.Equals(EnvironmentFileSuffix, StringComparison.OrdinalIgnoreCase) ||
        key.Equals(GetEnvironmentFileVariableName(rootContext), StringComparison.OrdinalIgnoreCase);
    
    public static string GetEnvironmentFileVariableName(string rootContext) =>
        $"{rootContext.ToUpper()}_{EnvironmentFileSuffix}";

    public static string? GetEnvironmentFilePath(string rootContext) => Environment
        .GetEnvironmentVariable(GetEnvironmentFileVariableName(rootContext));
}