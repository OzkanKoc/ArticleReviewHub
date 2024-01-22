using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Serializer;

namespace Infrastructure.Serializer;

public class SystemTextSerializer : ISerializer
{
    public JsonSerializerOptions SerializerOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    public string Serialize<T>(T value) => JsonSerializer.Serialize(value, SerializerOptions);

    public T Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value, SerializerOptions);

    public object Deserialize(string value, Type type) => JsonSerializer.Deserialize(value, type, SerializerOptions);
}
