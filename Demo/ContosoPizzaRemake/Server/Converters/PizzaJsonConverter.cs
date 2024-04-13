/////////////////////////////////////////////////////////////
// ContosoPizza.Models.PizzaJsonConverter                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-13T13:50:47.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using Net.Leksi.Pocota.Server;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models;


internal class PizzaJsonConverter: JsonConverter<Pizza>
{
    private readonly IServiceProvider _services;
    private readonly PizzaContext _dbContext;
    private readonly PocotaContext _context;
    public PizzaJsonConverter(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaContext>();
        _context = _services.GetRequiredService<PocotaContext>();

    }
    public override Pizza? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Pizza value, JsonSerializerOptions options)
    {
        PizzaPocota pocotaEntity = _context.Entity<PizzaPocota>(value);
        writer.WriteStartObject();
        writer.WritePropertyName(pocotaEntity.IsSerialized ? "$ref" : "$id");
        pocotaEntity.IsSerialized = true;
        writer.WriteNumberValue(pocotaEntity.PocotaId);
        if(!pocotaEntity.Id.IsSent)
        {
            pocotaEntity.Id.IsSent = true;
            writer.WritePropertyName("Id");
            writer.WriteStartArray();
            writer.WriteNumberValue((int)pocotaEntity.Id.Access);
            JsonSerializer.Serialize(writer, value.Id, options);
            writer.WriteEndArray();
        }
        if(!pocotaEntity.Name.IsSent)
        {
            pocotaEntity.Name.IsSent = true;
            writer.WritePropertyName("Name");
            writer.WriteStartArray();
            writer.WriteNumberValue((int)pocotaEntity.Name.Access);
            JsonSerializer.Serialize(writer, value.Name, options);
            writer.WriteEndArray();
        }
        if(!pocotaEntity.Sauce.IsSent)
        {
            pocotaEntity.Sauce.IsSent = true;
            writer.WritePropertyName("Sauce");
            writer.WriteStartArray();
            writer.WriteNumberValue((int)pocotaEntity.Sauce.Access);
            JsonSerializer.Serialize(writer, value.Sauce, options);
            writer.WriteEndArray();
        }
        if(!pocotaEntity.Toppings.IsSent)
        {
            pocotaEntity.Toppings.IsSent = true;
            writer.WritePropertyName("Toppings");
            writer.WriteStartArray();
            writer.WriteNumberValue((int)pocotaEntity.Toppings.Access);
            JsonSerializer.Serialize(writer, value.Toppings, options);
            writer.WriteEndArray();
        }
        writer.WriteEndObject();
    }
}