/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaController                            //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-11T18:57:55.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPizza;

[ApiController]
[Route("/Pizza")]

public class PizzaController: ControllerBase
{
    private JsonSerializerOptions? _serializerOptions;
    private readonly object _lock = new();
    [HttpGet("GetAllPizzas")]
    public async Task GetAllPizzas()
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            _storageService.GetAllPizzasAsync(), 
            _serializerOptions
        );
    }
    [HttpGet("FindPizzas/{filter}")]
    public async Task FindPizzas(string filter)
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        PizzaFilter _filter = JsonSerializer.Deserialize<PizzaFilter>(filter, _serializerOptions)!;
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            _storageService.FindPizzasAsync(_filter), 
            _serializerOptions
        );
    }
    [HttpGet("GetPizza/{pizza}")]
    public async Task GetPizza(string pizza)
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        Pizza _pizza = JsonSerializer.Deserialize<Pizza>(pizza, _serializerOptions)!;
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            await _storageService.GetPizzaAsync(_pizza), 
            _serializerOptions
        );
    }
    [HttpGet("GetAllSauces")]
    public async Task GetAllSauces()
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            _storageService.GetAllSaucesAsync(), 
            _serializerOptions
        );
    }
    [HttpGet("GetSauce/{sauce}")]
    public async Task GetSauce(string sauce)
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        Sauce _sauce = JsonSerializer.Deserialize<Sauce>(sauce, _serializerOptions)!;
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            await _storageService.GetSauceAsync(_sauce), 
            _serializerOptions
        );
    }
    [HttpGet("GetAllToppings")]
    public async Task GetAllToppings()
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            _storageService.GetAllToppingsAsync(), 
            _serializerOptions
        );
    }
    [HttpGet("GetTopping/{topping}")]
    public async Task GetTopping(string topping)
    {
        PizzaServiceBase _storageService = HttpContext.RequestServices.GetRequiredService<PizzaServiceBase>();
        JsonSerializerOptions _serializerOptions = GetJsonSerializerOptions(HttpContext.RequestServices);
        Topping _topping = JsonSerializer.Deserialize<Topping>(topping, _serializerOptions)!;
        await JsonSerializer.SerializeAsync(
            HttpContext.Response.Body, 
            await _storageService.GetToppingAsync(_topping), 
            _serializerOptions
        );
    }
    private JsonSerializerOptions GetJsonSerializerOptions(IServiceProvider services)
    {
        if(_serializerOptions is null)
        {
            lock(_lock){
                if(_serializerOptions is null)
                {
                    _serializerOptions = new(){
                          ReferenceHandler = ReferenceHandler.Preserve,
                    };
                    _serializerOptions.Converters.Add(
                        services.GetRequiredService<PizzaJsonConverterFactory>()
                    );
                }
            }
        }
        return _serializerOptions;
    }
}
