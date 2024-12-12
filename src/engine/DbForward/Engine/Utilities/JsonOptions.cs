using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbForward.Engine.Utilities;

public static class JsonExtensions
{
    public static readonly JsonSerializerOptions StandardOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string Json<T>(this T obj) where T : class => JsonSerializer.Serialize(obj, StandardOptions);

    public static async Task WriteJsonAsync<T>(this TextWriter textWriter, T obj) where T : class
    {
        await textWriter.WriteAsync(JsonSerializer.Serialize(obj, StandardOptions));
    }
}