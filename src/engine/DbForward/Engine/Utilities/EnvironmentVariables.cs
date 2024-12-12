namespace DbForward.Engine.Utilities;

internal static class EnvironmentVariables
{
    internal record Descriptor(string Name, string Description, string? Value);

    public static Descriptor EditorProgram => new(
        "DB4_EDITOR",
        "The name of the preferred text editor",
        Environment.GetEnvironmentVariable("DB4_EDITOR"));

    public static Descriptor EditorArgumentsFormat => new(
        "DB4_EDITOR_ARGS",
        "A string used to compose arguments passed to the text editor",
        Environment.GetEnvironmentVariable("DB4_EDITOR_ARGS"));

    public static IEnumerable<Descriptor> GetDescriptors() => new[]
    {
        EditorProgram,
        EditorArgumentsFormat
    };
}