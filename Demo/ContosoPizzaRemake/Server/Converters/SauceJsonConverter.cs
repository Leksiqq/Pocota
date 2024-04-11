/////////////////////////////////////////////////////////////
// ContosoPizza.Models.SauceJsonConverter                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-11T18:57:55.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models;


public class SauceJsonConverter: JsonConverter<Sauce>
{
    private readonly IServiceProvider _services;
    private readonly PizzaContext _dbContext;
    public SauceJsonConverter(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaContext>();
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
        writer.WritePropertyName("Name");
        JsonSerializer.Serialize(writer, value.Name, options);
        writer.WritePropertyName("IsVegan");
        JsonSerializer.Serialize(writer, value.IsVegan, options);
        writer.WriteEndObject();
    }
}