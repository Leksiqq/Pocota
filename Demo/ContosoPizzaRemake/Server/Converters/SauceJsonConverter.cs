/////////////////////////////////////////////////////////////
// ContosoPizza.Models.SauceJsonConverter                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-13T13:50:47.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using Net.Leksi.Pocota.Server;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models;


internal class SauceJsonConverter: JsonConverter<Sauce>
{
    private readonly IServiceProvider _services;
    private readonly PizzaContext _dbContext;
    private readonly PocotaContext _context;
    public SauceJsonConverter(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaContext>();
        _context = _services.GetRequiredService<PocotaContext>();

    }
    public override Sauce? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Sauce value, JsonSerializerOptions options)
    {
        SaucePocota pocotaEntity = _context.Entity<SaucePocota>(value);
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
        if(!pocotaEntity.IsVegan.IsSent)
        {
            pocotaEntity.IsVegan.IsSent = true;
            writer.WritePropertyName("IsVegan");
            writer.WriteStartArray();
            writer.WriteNumberValue((int)pocotaEntity.IsVegan.Access);
            JsonSerializer.Serialize(writer, value.IsVegan, options);
            writer.WriteEndArray();
        }
        writer.WriteEndObject();
    }
}