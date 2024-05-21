/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.ToppingJsonConverter         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-21T11:07:45.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using ContosoPizza.Client;
using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models.Client;


internal class ToppingJsonConverter: JsonConverter<Topping>
{
    private const string s_Id = "Id";
    private const string s_Name = "Name";
    private const string s_Calories = "Calories";
    private const string s_Pizzas = "Pizzas";
    private readonly IServiceProvider _services;
    private readonly PizzaPocotaContext _context;
    public ToppingJsonConverter(IServiceProvider services)
    {
        _services = services;
        _context = _services.GetRequiredService<PizzaPocotaContext>();
    }
    public override Topping? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Topping value, JsonSerializerOptions options)
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
    private void WriteKeyOnly(Utf8JsonWriter writer, Topping value, JsonSerializerOptions options)
    {
        IToppingPocotaEntity? pocotaEntity = PocotaContext.Entity<IToppingPocotaEntity>(value);
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
        if(!keysFilled || _context.IsKey(pocotaEntity.Calories)) 
        {
            writer.WritePropertyName(s_Calories);
            JsonSerializer.Serialize(writer, value.Calories, options);
        }
        if(!keysFilled || _context.IsKey(pocotaEntity.Pizzas)) 
        {
            writer.WritePropertyName(s_Pizzas);
            JsonSerializer.Serialize(writer, value.Pizzas, options);
        }
        writer.WriteEndObject();
    }
    private void WriteUpdateAll(Utf8JsonWriter writer, Topping value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}