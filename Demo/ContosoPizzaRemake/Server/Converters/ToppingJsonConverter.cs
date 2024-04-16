/////////////////////////////////////////////////////////////
// ContosoPizza.Models.ToppingJsonConverter                //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-16T16:51:07.                                 //
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
    private readonly PocotaContext _context;
    public ToppingJsonConverter(IServiceProvider services)
    {
        _services = services;
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
        ToppingPocotaEntity pocotaEntity = _context.Entity<ToppingPocotaEntity>(value);
        writer.WriteStartObject();
        writer.WritePropertyName(pocotaEntity.IsSerialized ? "$ref" : "$id");
        if (withFieldsAccess)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue((int)pocotaEntity.Access);
        }
        writer.WriteNumberValue(pocotaEntity.PocotaId);
        if (withFieldsAccess)
        {
            writer.WriteEndArray();
        }
        pocotaEntity.IsSerialized = true;
        if(!pocotaEntity.Id.IsSent)
        {
            if(pocotaEntity.Id.Access is not AccessKind.NotSet)
            {
                pocotaEntity.Id.IsSent = true;
                writer.WritePropertyName("Id");
                if(withFieldsAccess)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue((int)pocotaEntity.Id.Access);
                }
                if(
                    pocotaEntity.Id.Access is AccessKind.Hidden
                )
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, value.Id, options);
                }
                if(withFieldsAccess)
                {
                    writer.WriteEndArray();
                }
            }
        }
        if(!pocotaEntity.Name.IsSent)
        {
            if(pocotaEntity.Name.Access is not AccessKind.NotSet)
            {
                pocotaEntity.Name.IsSent = true;
                writer.WritePropertyName("Name");
                if(withFieldsAccess)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue((int)pocotaEntity.Name.Access);
                }
                if(
                    pocotaEntity.Name.Access is AccessKind.Hidden
                )
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, value.Name, options);
                }
                if(withFieldsAccess)
                {
                    writer.WriteEndArray();
                }
            }
        }
        if(!pocotaEntity.Calories.IsSent)
        {
            if(pocotaEntity.Calories.Access is not AccessKind.NotSet)
            {
                pocotaEntity.Calories.IsSent = true;
                writer.WritePropertyName("Calories");
                if(withFieldsAccess)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue((int)pocotaEntity.Calories.Access);
                }
                if(
                    pocotaEntity.Calories.Access is AccessKind.Hidden
                )
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, value.Calories, options);
                }
                if(withFieldsAccess)
                {
                    writer.WriteEndArray();
                }
            }
        }
        if(!pocotaEntity.Pizzas.IsSent)
        {
            if(pocotaEntity.Pizzas.Access is not AccessKind.NotSet)
            {
                pocotaEntity.Pizzas.IsSent = true;
                writer.WritePropertyName("Pizzas");
                if(withFieldsAccess)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue((int)pocotaEntity.Pizzas.Access);
                }
                if(
                    pocotaEntity.Pizzas.Access is AccessKind.Hidden
                )
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, value.Pizzas, options);
                }
                if(withFieldsAccess)
                {
                    writer.WriteEndArray();
                }
            }
        }
        writer.WriteEndObject();
    }
}