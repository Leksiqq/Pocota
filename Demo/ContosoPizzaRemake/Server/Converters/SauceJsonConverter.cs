/////////////////////////////////////////////////////////////
// ContosoPizza.Models.SauceJsonConverter                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-20T17:10:07.                                 //
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
    private const string s_Id = "Id";
    private const string s_Id1 = "Id1";
    private const string s_Name = "Name";
    private const string s_IsVegan = "IsVegan";
    private const string s_refName = "$ref";
    private const string s_idName = "$id";
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    public SauceJsonConverter(IServiceProvider services)
    {
        _services = services;
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
        SaucePocotaEntity pocotaEntity = _context.Entity<SaucePocotaEntity>(value);
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
        if(!pocotaEntity.Id1.IsSent)
        {
            if(pocotaEntity.Id1.Access is not AccessKind.NotSet)
            {
                pocotaEntity.Id1.IsSent = true;
                writer.WritePropertyName(s_Id1);
                if(withFieldsAccess)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue((int)pocotaEntity.Id1.Access);
                }
                if(
                    pocotaEntity.Id1.Access is AccessKind.Forbidden
                )
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, value.Id1, options);
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
        if(!pocotaEntity.IsVegan.IsSent)
        {
            if(pocotaEntity.IsVegan.Access is not AccessKind.NotSet)
            {
                pocotaEntity.IsVegan.IsSent = true;
                writer.WritePropertyName(s_IsVegan);
                if(withFieldsAccess)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue((int)pocotaEntity.IsVegan.Access);
                }
                if(
                    pocotaEntity.IsVegan.Access is AccessKind.Forbidden
                )
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, value.IsVegan, options);
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