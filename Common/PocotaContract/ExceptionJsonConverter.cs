using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Net.Leksi.Pocota.Contract;

public class ExceptionJsonConverter : JsonConverter<Exception>
{
    private const string ExceptionType = "ExceptionType";

    public Exception? Target { get; set; }

    public override Exception? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        Type resultType = typeof(Exception);
        string resultTypeName;

        Exception? exception = Target ?? new PocotaRemoteException(null);
        Target = null;

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndObject)
            {
                break;
            }
            if (reader.TokenType is not JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            string propertyName = reader.GetString()!;
            if (!reader.Read())
            {
                throw new JsonException();
            }
            if (ExceptionType.Equals(propertyName))
            {
                resultTypeName = reader.GetString()!;
                resultType = Type.GetType(resultTypeName) ?? typeof(Exception);
                exception.Data.Add(propertyName, resultTypeName);
            }
            else
            {
                JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
                PropertyInfo? propertyInfo = resultType.GetProperty(propertyName);
                bool isConverted = false;
                if(propertyInfo is { })
                {
                    if(propertyInfo.PropertyType.GetConstructor([]) is { })
                    {
                        exception.Data.Add(propertyName, JsonSerializer.Deserialize(jsonElement, propertyInfo.PropertyType, options));
                        isConverted = true;
                    }
                }
                if (!isConverted)
                {
                    exception.Data.Add(propertyName, Deserialize(jsonElement));
                }
            }
        }
        return exception;
    }

    private static object? Deserialize(JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Object:
                Dictionary<string, object> dict = [];
                foreach (var entry in jsonElement.EnumerateObject())
                {
                    dict.Add(entry.Name, Deserialize(entry.Value)!);
                }
                return dict;
            case JsonValueKind.Array:
                List<object> list = [];
                foreach (var entry in jsonElement.EnumerateArray())
                {
                    list.Add(Deserialize(entry)!);
                }
                return list;
            default:
                return JsonSerializer.Deserialize<object>(jsonElement);
        }
    }

    public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(ExceptionType);
        writer.WriteStringValue(value.GetType().FullName);
        foreach (PropertyInfo pi in value.GetType().GetProperties())
        {
            if (
                typeof(string) == pi.PropertyType 
                || typeof(Exception).IsAssignableFrom(pi.PropertyType)
                || pi.PropertyType.IsPrimitive
                || typeof(ICollection<Exception>).IsAssignableFrom(pi.PropertyType)
                || "Data".Equals(pi.Name)
            )
            {
                object? valueObj = pi.GetValue(value);
                if (valueObj is { })
                {
                    writer.WritePropertyName(pi.Name);
                    try
                    {
                        JsonSerializer.Serialize(writer, valueObj, pi.PropertyType, options);
                    }
                    catch
                    {
                        writer.WriteStringValue("<<<skipped>>>");
                    }
                }
            }
        }
        writer.WriteEndObject();
    }
}
