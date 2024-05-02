/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.SauceJsonConverter           //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-02T20:44:06.                                 //
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
    private const string s_Id = "Id";
    private const string s_Id1 = "Id1";
    private const string s_Name = "Name";
    private const string s_IsVegan = "IsVegan";
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
        ISaucePocotaEntity? pocotaEntity = PocotaContext.Entity<ISaucePocotaEntity>(value);
        if (pocotaEntity is null)
        {
            throw new InvalidOperationException();
        }
        bool keysFilled = _context.KeysFilled(value);
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