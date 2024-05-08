/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaJsonConverterFactory                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-08T11:28:37.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza;


internal class PizzaJsonConverterFactory(IServiceProvider services): JsonConverterFactory
{
    private static readonly Dictionary<Type, Type> _entityJsonConverterTypes = new()
    {
        {typeof(Pizza), typeof(PizzaJsonConverter)},
        {typeof(Sauce), typeof(SauceJsonConverter)},
        {typeof(Topping), typeof(ToppingJsonConverter)},
    };
    public override bool CanConvert(Type typeToConvert)
    {
        return _entityJsonConverterTypes.ContainsKey(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if(_entityJsonConverterTypes.TryGetValue(typeToConvert, out Type? converterType))
        {
            return services.GetRequiredService(converterType) as JsonConverter;
        }
        return null;
    }
}