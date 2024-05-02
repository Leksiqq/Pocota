/////////////////////////////////////////////////
// ContosoPizza.Client.PizzaConnector          //
// was generated automatically from            //
// at 2024-05-02T14:19:53.                     //
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
    private readonly PizzaPocotaContext _context;
    public PizzaConnector(IServiceProvider services): base(services) 
    {
        _context = _services.GetRequiredService<PizzaPocotaContext>();
        _serializerOptions.Converters.Add(
            _services.GetRequiredService<PizzaJsonConverterFactory>()
        );
    }
    public async Task GetAllPizzasAsync(ICollection<Pizza> target, CancellationToken cancellationToken) 
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try 
        {
            _context.ClearSentEntities();
            _context.KeyOnlyJson = true;
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
    public async Task FindPizzasAsync(ICollection<Pizza> target, PizzaFilter filter, CancellationToken cancellationToken) 
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try 
        {
            _context.ClearSentEntities();
            _context.KeyOnlyJson = true;
            string _filter = HttpUtility.UrlEncode(JsonSerializer.Serialize(filter, _serializerOptions));
            HttpRequestMessage _request = new(
                HttpMethod.Get, 
                $"/Pizza/FindPizzas/{_filter}"
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
            _context.ClearSentEntities();
            _context.KeyOnlyJson = true;
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
            _context.ClearSentEntities();
            _context.KeyOnlyJson = true;
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
            _context.ClearSentEntities();
            _context.KeyOnlyJson = true;
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
            _context.ClearSentEntities();
            _context.KeyOnlyJson = true;
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
            _context.ClearSentEntities();
            _context.KeyOnlyJson = true;
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
    public async Task UpdateAll(CancellationToken cancellationToken)
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try
        {
            _context.ClearSentEntities();
            _context.KeyOnlyJson = false;
            HttpRequestMessage _request = new(HttpMethod.Post, "/Pizza");
            await Task.CompletedTask;
        }
        finally
        {
            _asyncLock.Release();
        }
    }
}