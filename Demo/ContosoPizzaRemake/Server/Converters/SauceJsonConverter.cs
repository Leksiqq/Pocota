/////////////////////////////////////////////////////////////
// ContosoPizza.Models.SauceJsonConverter                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-14T15:28:52.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using Microsoft.AspNetCore.Http;
using Net.Leksi.Pocota.Contract;
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
        bool withFieldsAccess = _services.GetRequiredService<IHttpContextAccessor>().HttpContext!
            .Request.Headers.ContainsKey(PocotaHeader.WithFieldsAccess);
        SaucePocota pocotaEntity = _context.Entity<SaucePocota>(value);
        writer.WriteStartObject();
        writer.WritePropertyName(pocotaEntity.IsSerialized ? "$ref" : "$id");
        pocotaEntity.IsSerialized = true;
        writer.WriteNumberValue(pocotaEntity.PocotaId);
        if(!pocotaEntity.Id.IsSent)
        {
            pocotaEntity.Id.IsSent = true;
            writer.WritePropertyName("Id");
            if(withFieldsAccess)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue((int)pocotaEntity.Id.Access);
            }
            JsonSerializer.Serialize(writer, value.Id, options);
            if(withFieldsAccess)
            {
                writer.WriteEndArray();
            }
        }
        if(!pocotaEntity.Name.IsSent)
        {
            pocotaEntity.Name.IsSent = true;
            writer.WritePropertyName("Name");
            if(withFieldsAccess)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue((int)pocotaEntity.Name.Access);
            }
            JsonSerializer.Serialize(writer, value.Name, options);
            if(withFieldsAccess)
            {
                writer.WriteEndArray();
            }
        }
        if(!pocotaEntity.IsVegan.IsSent)
        {
            pocotaEntity.IsVegan.IsSent = true;
            writer.WritePropertyName("IsVegan");
            if(withFieldsAccess)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue((int)pocotaEntity.IsVegan.Access);
            }
            JsonSerializer.Serialize(writer, value.IsVegan, options);
            if(withFieldsAccess)
            {
                writer.WriteEndArray();
            }
        }
        writer.WriteEndObject();
    }
}