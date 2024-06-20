using System.Text.Json;
using System.Text.Json.Serialization;

namespace Net.Leksi.Pocota.Client;

public class CommonJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if(typeToConvert == typeof(Type))
        {
            return true;
        }
        return false;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(Type))
        {
            return new TypeConverter();
        }
        return null;
    }

    private class TypeConverter : JsonConverter<Type>
    {
        public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteStringValue(Util.BuildTypeFullName(value));
            writer.WriteEndObject();
        }
    }
}
