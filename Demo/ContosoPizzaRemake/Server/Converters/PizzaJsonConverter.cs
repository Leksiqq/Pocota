/////////////////////////////////////////////////////////////
// ContosoPizza.Models.PizzaJsonConverter                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-12T13:40:57.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using Net.Leksi.Pocota.Server;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models;


public class PizzaJsonConverter: JsonConverter<Pizza>
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
        Console.WriteLine(_dbContext.Entry(value).Properties.Count());
        var prop = _dbContext.Entry(value).Properties.Where(p => p.Metadata.Name == "Sauce").FirstOrDefault();
        //Console.WriteLine($"{prop.Metadata.Name}, o: {prop?.OriginalValue}, c: {prop?.CurrentValue}");
        PocotaEntity pocotaEntity = _context.Entity(value);
        writer.WriteStartObject();
        writer.WritePropertyName(pocotaEntity.IsSerialized ? "$ref" : "$id");
        pocotaEntity.SetSerialized();
        writer.WriteNumberValue(pocotaEntity.PocotaId);
        if(!pocotaEntity.IsPropertySent(0))
        {
            pocotaEntity.MarkPropertySent(0);
            writer.WritePropertyName("Id");
            JsonSerializer.Serialize(writer, value.Id, options);
        }
        if(!pocotaEntity.IsPropertySent(1))
        {
            pocotaEntity.MarkPropertySent(1);
            writer.WritePropertyName("Name");
            JsonSerializer.Serialize(writer, value.Name, options);
        }
        if(!pocotaEntity.IsPropertySent(2))
        {
            pocotaEntity.MarkPropertySent(2);
            writer.WritePropertyName("Sauce");
            JsonSerializer.Serialize(writer, value.Sauce, options);
        }
        if(!pocotaEntity.IsPropertySent(3))
        {
            pocotaEntity.MarkPropertySent(3);
            writer.WritePropertyName("Toppings");
            JsonSerializer.Serialize(writer, value.Toppings, options);
        }
        writer.WriteEndObject();
    }
}