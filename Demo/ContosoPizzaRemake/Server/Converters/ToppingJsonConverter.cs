/////////////////////////////////////////////////////////////
// ContosoPizza.Models.ToppingJsonConverter                //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-14T15:28:52.                                 //
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


internal class ToppingJsonConverter: JsonConverter<Topping>
{
    private readonly IServiceProvider _services;
    private readonly PizzaContext _dbContext;
    private readonly PocotaContext _context;
    public ToppingJsonConverter(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaContext>();
        _context = _services.GetRequiredService<PocotaContext>();

    }
    public override Topping? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Topping value, JsonSerializerOptions options)
    {
        bool withFieldsAccess = _services.GetRequiredService<IHttpContextAccessor>().HttpContext!
            .Request.Headers.ContainsKey(PocotaHeader.WithFieldsAccess);
        ToppingPocota pocotaEntity = _context.Entity<ToppingPocota>(value);
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
        if(!pocotaEntity.Calories.IsSent)
        {
            pocotaEntity.Calories.IsSent = true;
            writer.WritePropertyName("Calories");
            if(withFieldsAccess)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue((int)pocotaEntity.Calories.Access);
            }
            JsonSerializer.Serialize(writer, value.Calories, options);
            if(withFieldsAccess)
            {
                writer.WriteEndArray();
            }
        }
        if(!pocotaEntity.Pizzas.IsSent)
        {
            pocotaEntity.Pizzas.IsSent = true;
            writer.WritePropertyName("Pizzas");
            if(withFieldsAccess)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue((int)pocotaEntity.Pizzas.Access);
            }
            JsonSerializer.Serialize(writer, value.Pizzas, options);
            if(withFieldsAccess)
            {
                writer.WriteEndArray();
            }
        }
        writer.WriteEndObject();
    }
}