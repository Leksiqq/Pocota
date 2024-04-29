/////////////////////////////////////////////////
// ContosoPizza.Client.PizzaConnector          //
// was generated automatically from            //
// at 2024-04-29T17:17:24.                     //
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

namespace ContosoPizza.Client;


public class PizzaConnector: Connector
{
    private readonly SemaphoreSlim _asyncLock = new(1, 1);
    private readonly PizzaPocotaContext _context;
    private readonly JsonSerializerOptions _serializerOptions = new();
    public PizzaConnector(IServiceProvider services): base(services) 
    {
        _context = _services.GetRequiredService<PizzaPocotaContext>();
        _serializerOptions.Converters.Add(
            _services.GetRequiredService<PizzaJsonConverterFactory>()
        );
    }
    public async Task GetAllPizzasAsync(ICollection<Pizza> target) 
    {
        await _asyncLock.WaitAsync();
        try 
        {
            _context.ClearSentEntities();
            HttpRequestMessage _request = new(HttpMethod.Get, "/Pizza/GetAllPizzas");
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async Task FindPizzasAsync(ICollection<Pizza> target, PizzaFilter filter) 
    {
        await _asyncLock.WaitAsync();
        try 
        {
            _context.ClearSentEntities();
            HttpRequestMessage _request = new(HttpMethod.Get, "/Pizza/FindPizzas");
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async ValueTask<Pizza> GetPizzaAsync(Pizza pizza) 
    {
        await _asyncLock.WaitAsync();
        try 
        {
            _context.ClearSentEntities();
            HttpRequestMessage _request = new(HttpMethod.Get, "/Pizza/GetPizza");
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async Task GetAllSaucesAsync(ICollection<Sauce> target) 
    {
        await _asyncLock.WaitAsync();
        try 
        {
            _context.ClearSentEntities();
            HttpRequestMessage _request = new(HttpMethod.Get, "/Pizza/GetAllSauces");
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async ValueTask<Sauce> GetSauceAsync(Sauce sauce) 
    {
        await _asyncLock.WaitAsync();
        try 
        {
            _context.ClearSentEntities();
            HttpRequestMessage _request = new(HttpMethod.Get, "/Pizza/GetSauce");
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async Task GetAllToppingsAsync(ICollection<Topping> target) 
    {
        await _asyncLock.WaitAsync();
        try 
        {
            _context.ClearSentEntities();
            HttpRequestMessage _request = new(HttpMethod.Get, "/Pizza/GetAllToppings");
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async ValueTask<Topping> GetToppingAsync(Topping topping) 
    {
        await _asyncLock.WaitAsync();
        try 
        {
            _context.ClearSentEntities();
            HttpRequestMessage _request = new(HttpMethod.Get, "/Pizza/GetTopping");
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async Task UpdateAll() 
    {
        await _asyncLock.WaitAsync();
        try
        {
            _context.ClearSentEntities();
            HttpRequestMessage _request = new(HttpMethod.Post, "/Pizza");
            await Task.CompletedTask;
        }
        finally
        {
            _asyncLock.Release();
        }
    }
}