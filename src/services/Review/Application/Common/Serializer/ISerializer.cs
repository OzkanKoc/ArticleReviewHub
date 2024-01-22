using System.Text.Json;

namespace Application.Common.Serializer;

public interface ISerializer
{
    JsonSerializerOptions SerializerOptions { get; }
    string Serialize<T>(T value);
    T Deserialize<T>(string value);
    object Deserialize(string value, Type type);
}
