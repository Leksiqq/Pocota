/////////////////////////////////////////////////
// ContosoPizza.Client.PizzaConnector          //
// was generated automatically from            //
// at 2024-06-10T13:02:53.                     //
// Modifying this file will break the program! //
/////////////////////////////////////////////////

using ContosoPizza;
using ContosoPizza.Models.Client;
using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ContosoPizza.Client;


public class PizzaConnector: Connector
{
    private readonly SemaphoreSlim _asyncLock = new(1, 1);
    public PizzaConnector(IServiceProvider services, string serviceKey, Uri? baseUri): base(services, serviceKey, baseUri) 
    {
        _context = _services.GetRequiredService<PizzaPocotaContext>();
        _serializerOptions.Converters.Add(
            _services.GetRequiredService<PizzaJsonConverterFactory>()
        );
    }
    public override async Task GetPocotaConfigAsync(CancellationToken cancellationToken)
    {
        await GetPocotaConfigAsync("/Pizza", cancellationToken);
    }
    public async Task GetAllPizzasAsync(ICollection<Pizza> target, CancellationToken cancellationToken) 
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try 
        {
            ResetContext();
            HttpRequestMessage _request = new(
                HttpMethod.Get, 
                $"/Pizza/GetAllPizzas"
            );
            await GetResponseAsyncEnumerable<Pizza>(target, _request, _serializerOptions, cancellationToken);
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async Task FindPizzasAsync(ICollection<Pizza> target, PizzaFilter filter, Int32 stage, Boolean? sure, CancellationToken cancellationToken) 
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try 
        {
            ResetContext();
            string _filter = HttpUtility.UrlEncode(JsonSerializer.Serialize(filter, _serializerOptions));
            string _stage = HttpUtility.UrlEncode(JsonSerializer.Serialize(stage, _serializerOptions));
            string _sure = HttpUtility.UrlEncode(JsonSerializer.Serialize(sure, _serializerOptions));
            HttpRequestMessage _request = new(
                HttpMethod.Get, 
                $"/Pizza/FindPizzas/{_filter}/{_stage}/{_sure}"
            );
            await GetResponseAsyncEnumerable<Pizza>(target, _request, _serializerOptions, cancellationToken);
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async ValueTask<Pizza?> GetPizzaAsync(Pizza pizza, CancellationToken cancellationToken) 
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try 
        {
            ResetContext();
            string _pizza = HttpUtility.UrlEncode(JsonSerializer.Serialize(pizza, _serializerOptions));
            HttpRequestMessage _request = new(
                HttpMethod.Get, 
                $"/Pizza/GetPizza/{_pizza}"
            );
            return await GetResponseAsync<Pizza>(_request, _serializerOptions, cancellationToken);
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async Task GetAllSaucesAsync(ICollection<Sauce> target, CancellationToken cancellationToken) 
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try 
        {
            ResetContext();
            HttpRequestMessage _request = new(
                HttpMethod.Get, 
                $"/Pizza/GetAllSauces"
            );
            await GetResponseAsyncEnumerable<Sauce>(target, _request, _serializerOptions, cancellationToken);
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async ValueTask<Sauce?> GetSauceAsync(Sauce sauce, CancellationToken cancellationToken) 
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try 
        {
            ResetContext();
            string _sauce = HttpUtility.UrlEncode(JsonSerializer.Serialize(sauce, _serializerOptions));
            HttpRequestMessage _request = new(
                HttpMethod.Get, 
                $"/Pizza/GetSauce/{_sauce}"
            );
            return await GetResponseAsync<Sauce>(_request, _serializerOptions, cancellationToken);
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async Task GetAllToppingsAsync(ICollection<Topping> target, CancellationToken cancellationToken) 
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try 
        {
            ResetContext();
            HttpRequestMessage _request = new(
                HttpMethod.Get, 
                $"/Pizza/GetAllToppings"
            );
            await GetResponseAsyncEnumerable<Topping>(target, _request, _serializerOptions, cancellationToken);
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async ValueTask<Topping?> GetToppingAsync(Topping topping, CancellationToken cancellationToken) 
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try 
        {
            ResetContext();
            string _topping = HttpUtility.UrlEncode(JsonSerializer.Serialize(topping, _serializerOptions));
            HttpRequestMessage _request = new(
                HttpMethod.Get, 
                $"/Pizza/GetTopping/{_topping}"
            );
            return await GetResponseAsync<Topping>(_request, _serializerOptions, cancellationToken);
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async Task UpdateAllAsync(CancellationToken cancellationToken)
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try
        {
            ResetContext();
            HttpRequestMessage _request = new(HttpMethod.Post, "/Pizza");
            await Task.CompletedTask;
        }
        finally
        {
            _asyncLock.Release();
        }
    }
}