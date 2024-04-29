/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.ToppingJsonConverter         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-29T15:06:28.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models.Client;


internal class ToppingJsonConverter: JsonConverter<Topping>
{
    private readonly IServiceProvider _services;
    public ToppingJsonConverter(IServiceProvider services)
    {
        _services = services;

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