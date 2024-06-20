/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.ToppingJsonConverter         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-20T18:20:03.                                 //
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
    private const string s_Id = nameof(Topping.Id);
    private const string s_Name = nameof(Topping.Name);
    private const string s_Calories = nameof(Topping.Calories);
    private const string s_Pizzas = nameof(Topping.Pizzas);
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
        IToppingPocotaEntity pocotaEntity = (IToppingPocotaEntity)((IEntityOwner)value).Entity;
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
        writer.WriteEndObject();
    }
    private void WriteUpdateAll(Utf8JsonWriter writer, Topping value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}