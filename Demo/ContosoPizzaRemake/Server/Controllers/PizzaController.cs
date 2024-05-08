/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaController                            //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-08T11:28:37.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Microsoft.AspNetCore.Mvc;
using Net.Leksi.Pocota.Server;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza;

[ApiController]
[Route("/Pizza")]

public class PizzaController: ControllerBase
{
    [HttpGet("GetAllPizzas")]
    public async Task GetAllPizzas()
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        IAccessCalculator accessCalculator = HttpContext.RequestServices.GetRequiredKeyedService<IAccessCalculator>(typeof(Pizza));
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        HttpContext.Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            PocotaContext.ProcessEntitiesAsync<Pizza>(accessCalculator, _storageService.GetAllPizzasAsync()), 
            _serializerOptions
        );

    }
    [HttpGet("FindPizzas/{filter}")]
    public async Task FindPizzas(string filter)
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        IAccessCalculator accessCalculator = HttpContext.RequestServices.GetRequiredKeyedService<IAccessCalculator>(typeof(Pizza));
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        HttpContext.Response.ContentType = "application/json";
        PizzaFilter _filterFilter = JsonSerializer.Deserialize<PizzaFilter>(filter, _serializerOptions)!;
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            PocotaContext.ProcessEntitiesAsync<Pizza>(accessCalculator, _storageService.FindPizzasAsync(_filterFilter)), 
            _serializerOptions
        );

    }
    [HttpGet("GetPizza/{pizza}")]
    public async Task GetPizza(string pizza)
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        IAccessCalculator accessCalculator = HttpContext.RequestServices.GetRequiredKeyedService<IAccessCalculator>(typeof(Pizza));
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        HttpContext.Response.ContentType = "application/json";
        Pizza _pizzaFilter = JsonSerializer.Deserialize<Pizza>(pizza, _serializerOptions)!;
        if(await _storageService.GetPizzaAsync(_pizzaFilter) is Pizza _PizzaResult)
        {
            await JsonSerializer.SerializeAsync(
                HttpContext.Response.Body, 
                PocotaContext.ProcessEntity<Pizza>(accessCalculator, _PizzaResult), 
                _serializerOptions
            );
        }
        else 
        {
            NotFound();
        }

    }
    [HttpGet("GetAllSauces")]
    public async Task GetAllSauces()
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        IAccessCalculator accessCalculator = HttpContext.RequestServices.GetRequiredKeyedService<IAccessCalculator>(typeof(Sauce));
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        HttpContext.Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            PocotaContext.ProcessEntitiesAsync<Sauce>(accessCalculator, _storageService.GetAllSaucesAsync()), 
            _serializerOptions
        );

    }
    [HttpGet("GetSauce/{sauce}")]
    public async Task GetSauce(string sauce)
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        IAccessCalculator accessCalculator = HttpContext.RequestServices.GetRequiredKeyedService<IAccessCalculator>(typeof(Sauce));
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        HttpContext.Response.ContentType = "application/json";
        Sauce _sauceFilter = JsonSerializer.Deserialize<Sauce>(sauce, _serializerOptions)!;
        if(await _storageService.GetSauceAsync(_sauceFilter) is Sauce _SauceResult)
        {
            await JsonSerializer.SerializeAsync(
                HttpContext.Response.Body, 
                PocotaContext.ProcessEntity<Sauce>(accessCalculator, _SauceResult), 
                _serializerOptions
            );
        }
        else 
        {
            NotFound();
        }

    }
    [HttpGet("GetAllToppings")]
    public async Task GetAllToppings()
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        IAccessCalculator accessCalculator = HttpContext.RequestServices.GetRequiredKeyedService<IAccessCalculator>(typeof(Topping));
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        HttpContext.Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            PocotaContext.ProcessEntitiesAsync<Topping>(accessCalculator, _storageService.GetAllToppingsAsync()), 
            _serializerOptions
        );

    }
    [HttpGet("GetTopping/{topping}")]
    public async Task GetTopping(string topping)
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        IAccessCalculator accessCalculator = HttpContext.RequestServices.GetRequiredKeyedService<IAccessCalculator>(typeof(Topping));
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        HttpContext.Response.ContentType = "application/json";
        Topping _toppingFilter = JsonSerializer.Deserialize<Topping>(topping, _serializerOptions)!;
        if(await _storageService.GetToppingAsync(_toppingFilter) is Topping _ToppingResult)
        {
            await JsonSerializer.SerializeAsync(
                HttpContext.Response.Body, 
                PocotaContext.ProcessEntity<Topping>(accessCalculator, _ToppingResult), 
                _serializerOptions
            );
        }
        else 
        {
            NotFound();
        }

    }
    [HttpPost]
    public async Task UpdateAll() {
        await Task.CompletedTask;
    }
    private static JsonSerializerOptions GetJsonSerializerOptions(IServiceProvider services)
    {
        JsonSerializerOptions _serializerOptions = new();
        _serializerOptions.Converters.Add(
            services.GetRequiredService<PizzaJsonConverterFactory>()
        );
        return _serializerOptions;
    }
}
