/////////////////////////////////////////////////////////////
// ContosoPizza.Models.PizzaJsonConverter                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-14T15:28:51.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using Microsoft.AspNetCore.Http;
using Net.Leksi.Pocota.Contract;
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
        bool withFieldsAccess = _services.GetRequiredService<IHttpContextAccessor>().HttpContext!
            .Request.Headers.ContainsKey(PocotaHeader.WithFieldsAccess);
        PizzaPocota pocotaEntity = _context.Entity<PizzaPocota>(value);
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
        if(!pocotaEntity.Sauce.IsSent)
        {
            pocotaEntity.Sauce.IsSent = true;
            writer.WritePropertyName("Sauce");
            if(withFieldsAccess)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue((int)pocotaEntity.Sauce.Access);
            }
            JsonSerializer.Serialize(writer, value.Sauce, options);
            if(withFieldsAccess)
            {
                writer.WriteEndArray();
            }
        }
        if(!pocotaEntity.Toppings.IsSent)
        {
            pocotaEntity.Toppings.IsSent = true;
            writer.WritePropertyName("Toppings");
            if(withFieldsAccess)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue((int)pocotaEntity.Toppings.Access);
            }
            JsonSerializer.Serialize(writer, value.Toppings, options);
            if(withFieldsAccess)
            {
                writer.WriteEndArray();
            }
        }
        writer.WriteEndObject();
    }
}