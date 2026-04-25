using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManager.Mcp.Utilities.Serializers;

public class JsonOutputSerializer : IOutputSerializer
{
    private static readonly JsonSerializerOptions _options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
    };

    public string Serialize<T>(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return JsonSerializer.Serialize(value, _options);
    }
}
