/////////////////////////////////////////////////////////////
// ContosoPizza.Models.PizzaJsonConverter                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-30T14:05:55.                                 //
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
    private const string s_Id = "Id";
    private const string s_Name = "Name";
    private const string s_Sauce = "Sauce";
    private const string s_Toppings = "Toppings";
    private const string s_refName = "$ref";
    private const string s_idName = "$id";
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    public PizzaJsonConverter(IServiceProvider services)
    {
        _services = services;
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
        PizzaPocotaEntity pocotaEntity = _context.Entity<PizzaPocotaEntity>(value);
        writer.WriteStartObject();
        writer.WritePropertyName(pocotaEntity.IsSerialized ? s_refName : s_idName);
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
                writer.WritePropertyName(s_Id);
                if(withFieldsAccess)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue((int)pocotaEntity.Id.Access);
                }
                if(
                    pocotaEntity.Id.Access is AccessKind.Forbidden
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
                writer.WritePropertyName(s_Name);
                if(withFieldsAccess)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue((int)pocotaEntity.Name.Access);
                }
                if(
                    pocotaEntity.Name.Access is AccessKind.Forbidden
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
        if(!pocotaEntity.Sauce.IsSent)
        {
            if(pocotaEntity.Sauce.Access is not AccessKind.NotSet)
            {
                pocotaEntity.Sauce.IsSent = true;
                writer.WritePropertyName(s_Sauce);
                if(withFieldsAccess)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue((int)pocotaEntity.Sauce.Access);
                }
                if(
                    pocotaEntity.Sauce.Access is AccessKind.Forbidden
                )
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, value.Sauce, options);
                }
                if(withFieldsAccess)
                {
                    writer.WriteEndArray();
                }
            }
        }
        if(!pocotaEntity.Toppings.IsSent)
        {
            if(pocotaEntity.Toppings.Access is not AccessKind.NotSet)
            {
                pocotaEntity.Toppings.IsSent = true;
                writer.WritePropertyName(s_Toppings);
                if(withFieldsAccess)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue((int)pocotaEntity.Toppings.Access);
                }
                if(
                    pocotaEntity.Toppings.Access is AccessKind.Forbidden
                )
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, value.Toppings, options);
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