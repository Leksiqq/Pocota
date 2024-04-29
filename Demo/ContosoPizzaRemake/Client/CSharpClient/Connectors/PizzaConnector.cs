/////////////////////////////////////////////////
// ContosoPizza.Client.PizzaConnector          //
// was generated automatically from            //
// at 2024-04-29T15:06:27.                     //
// Modifying this file will break the program! //
/////////////////////////////////////////////////

using ContosoPizza;
using ContosoPizza.Models.Client;
using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContosoPizza.Client;


public class PizzaConnector: Connector
{
    private readonly SemaphoreSlim _asyncLock = new(1, 1);
    private readonly PizzaPocotaContext _context;
    public PizzaConnector(IServiceProvider services): base(services) 
    {
        _context = _services.GetRequiredService<PizzaPocotaContext>();
    }
    public async Task GetAllPizzasAsync(ICollection<Pizza> target) 
    {
        await _asyncLock.WaitAsync();
        try 
        {
            _context.ClearSentEntities();
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
            await Task.CompletedTask;
        }
        finally
        {
            _asyncLock.Release();
        }
    }
}