/////////////////////////////////////////////////////////////
// ContosoPizza.Models.ToppingJsonConverter                //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-11T18:57:55.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models;


public class ToppingJsonConverter: JsonConverter<Topping>
{
    private readonly IServiceProvider _services;
    private readonly PizzaContext _dbContext;
    public ToppingJsonConverter(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaContext>();
    }
    public override Topping? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Topping value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Id");
        JsonSerializer.Serialize(writer, value.Id, options);
        writer.WritePropertyName("Name");
        JsonSerializer.Serialize(writer, value.Name, options);
        writer.WritePropertyName("Calories");
        JsonSerializer.Serialize(writer, value.Calories, options);
        writer.WritePropertyName("Pizzas");
        JsonSerializer.Serialize(writer, value.Pizzas, options);
        writer.WriteEndObject();
    }
}