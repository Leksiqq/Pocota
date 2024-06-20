/////////////////////////////////////////////////
// ContosoPizza.Client.PizzaConnector          //
// was generated automatically from            //
// at 2024-06-20T18:20:03.                     //
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
        _methodsOptionsTypes.Add(((Delegate)GetAllPizzasAsync).Method, null);
        _methodsOptionsTypes.Add(((Delegate)FindPizzasAsync).Method, typeof(FindPizzasOptions));
        _methodsOptionsTypes.Add(((Delegate)GetPizzaAsync).Method, typeof(Pizza));
        _methodsOptionsTypes.Add(((Delegate)GetAllSaucesAsync).Method, null);
        _methodsOptionsTypes.Add(((Delegate)GetSauceAsync).Method, typeof(Sauce));
        _methodsOptionsTypes.Add(((Delegate)GetAllToppingsAsync).Method, null);
        _methodsOptionsTypes.Add(((Delegate)GetToppingAsync).Method, typeof(Topping));
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
            await GetResponseAsyncEnumerable(target, _request, _serializerOptions, cancellationToken);
        }
        finally 
        {
            _asyncLock.Release();
        }
    }
    public async Task FindPizzasAsync(ICollection<Pizza> target, FindPizzasOptions options, CancellationToken cancellationToken) 
    {
        await _asyncLock.WaitAsync(cancellationToken);
        try 
        {
            ResetContext();
            string _optionsFilter = HttpUtility.UrlEncode(JsonSerializer.Serialize(options.Filter, _serializerOptions));
            string _optionsStage = HttpUtility.UrlEncode(JsonSerializer.Serialize(options.Stage, _serializerOptions));
            string _optionsSure = HttpUtility.UrlEncode(JsonSerializer.Serialize(options.Sure, _serializerOptions));

            HttpRequestMessage _request = new(
                HttpMethod.Get,
                $"/Pizza/FindPizzas/{_optionsFilter}/{_optionsStage}/{_optionsSure}"
            );
            await GetResponseAsyncEnumerable(target, _request, _serializerOptions, cancellationToken);
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
            await GetResponseAsyncEnumerable(target, _request, _serializerOptions, cancellationToken);
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
            await GetResponseAsyncEnumerable(target, _request, _serializerOptions, cancellationToken);
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