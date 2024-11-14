using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnitTests;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions Value = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}