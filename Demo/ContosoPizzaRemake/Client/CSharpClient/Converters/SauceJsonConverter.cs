/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.SauceJsonConverter           //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-20T18:20:03.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using ContosoPizza.Client;
using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models.Client;


internal class SauceJsonConverter: JsonConverter<Sauce>
{
    private const string s_Id = nameof(Sauce.Id);
    private const string s_Id1 = nameof(Sauce.Id1);
    private const string s_Name = nameof(Sauce.Name);
    private const string s_IsVegan = nameof(Sauce.IsVegan);
    private readonly IServiceProvider _services;
    private readonly PizzaPocotaContext _context;
    public SauceJsonConverter(IServiceProvider services)
    {
        _services = services;
        _context = _services.GetRequiredService<PizzaPocotaContext>();
    }
    public override Sauce? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Sauce value, JsonSerializerOptions options)
    {
        if(_context.KeyOnlyJson) 
        {
            WriteKeyOnly(writer, value, options);
        }
        else 
        {
            WriteUpdateAll(writer, value, options);
        }
    }
    private void WriteKeyOnly(Utf8JsonWriter writer, Sauce value, JsonSerializerOptions options)
    {
        ISaucePocotaEntity pocotaEntity = (ISaucePocotaEntity)((IEntityOwner)value).Entity;
        if (pocotaEntity is null)
        {
            throw new InvalidOperationException();
        }
        bool keysFilled = _context.KeysFilled(pocotaEntity);
        writer.WriteStartObject();
        if(!keysFilled || _context.IsKey(pocotaEntity.Id)) 
        {
            writer.WritePropertyName(s_Id);
            JsonSerializer.Serialize(writer, value.Id, options);
        }
        if(!keysFilled || _context.IsKey(pocotaEntity.Id1)) 
        {
            writer.WritePropertyName(s_Id1);
            JsonSerializer.Serialize(writer, value.Id1, options);
        }
        if(!keysFilled || _context.IsKey(pocotaEntity.Name)) 
        {
            writer.WritePropertyName(s_Name);
            JsonSerializer.Serialize(writer, value.Name, options);
        }
        if(!keysFilled || _context.IsKey(pocotaEntity.IsVegan)) 
        {
            writer.WritePropertyName(s_IsVegan);
            JsonSerializer.Serialize(writer, value.IsVegan, options);
        }
        writer.WriteEndObject();
    }
    private void WriteUpdateAll(Utf8JsonWriter writer, Sauce value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}