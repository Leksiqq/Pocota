/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.SauceJsonConverter           //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-29T17:17:24.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models.Client;


internal class SauceJsonConverter: JsonConverter<Sauce>
{
    private readonly IServiceProvider _services;
    public SauceJsonConverter(IServiceProvider services)
    {
        _services = services;

    }
    public override Sauce? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Sauce value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Id");
        JsonSerializer.Serialize(writer, value.Id, options);
        writer.WritePropertyName("Id1");
        JsonSerializer.Serialize(writer, value.Id1, options);
        writer.WritePropertyName("Name");
        JsonSerializer.Serialize(writer, value.Name, options);
        writer.WritePropertyName("IsVegan");
        JsonSerializer.Serialize(writer, value.IsVegan, options);
        writer.WriteEndObject();
    }
}