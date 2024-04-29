/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.PizzaJsonConverter           //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-29T17:17:24.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using ContosoPizza.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models.Client;


internal class PizzaJsonConverter: JsonConverter<Pizza>
{
    private readonly IServiceProvider _services;
    public PizzaJsonConverter(IServiceProvider services)
    {
        _services = services;

    }
    public override Pizza? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Pizza value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Id");
        JsonSerializer.Serialize(writer, value.Id, options);
        writer.WritePropertyName("Name");
        JsonSerializer.Serialize(writer, value.Name, options);
        writer.WritePropertyName("Sauce");
        JsonSerializer.Serialize(writer, value.Sauce, options);
        writer.WritePropertyName("Toppings");
        JsonSerializer.Serialize(writer, value.Toppings, options);
        writer.WriteEndObject();
    }
}