/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.PizzaJsonConverter           //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-29T18:20:46.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using ContosoPizza.Client;
using ContosoPizza.Models;
using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models.Client;


internal class PizzaJsonConverter: JsonConverter<Pizza>
{
    private const string s_Id = "Id";
    private const string s_Name = "Name";
    private const string s_Sauce = "Sauce";
    private const string s_Toppings = "Toppings";
    private readonly IServiceProvider _services;
    private readonly PizzaPocotaContext _context;
    public PizzaJsonConverter(IServiceProvider services)
    {
        _services = services;
        _context = _services.GetRequiredService<PizzaPocotaContext>();
    }
    public override Pizza? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Pizza value, JsonSerializerOptions options)
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
    private void WriteKeyOnly(Utf8JsonWriter writer, Pizza value, JsonSerializerOptions options)
    {
        IPizzaPocotaEntity? pocotaEntity = PocotaContext.Entity<IPizzaPocotaEntity>(value);
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
        if(!keysFilled || _context.IsKey(pocotaEntity.Name)) 
        {
            writer.WritePropertyName(s_Name);
            JsonSerializer.Serialize(writer, value.Name, options);
        }
        if(!keysFilled || _context.IsKey(pocotaEntity.Sauce)) 
        {
            writer.WritePropertyName(s_Sauce);
            JsonSerializer.Serialize(writer, value.Sauce, options);
        }
        if(!keysFilled || _context.IsKey(pocotaEntity.Toppings)) 
        {
            writer.WritePropertyName(s_Toppings);
            JsonSerializer.Serialize(writer, value.Toppings, options);
        }
        writer.WriteEndObject();
    }
    private void WriteUpdateAll(Utf8JsonWriter writer, Pizza value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}