/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaExtensions                     //
// was generated automatically from ContosoPizza.IContract //
// at 2024-07-31T17:13:13.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models.Client;
using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client;

namespace ContosoPizza.Client;


public static class PizzaExtensions
{
    public static IServiceCollection AddPizza(
        this IServiceCollection services,
        string name = "Pizza",
        Uri? baseUri = null
    )
    {
        services.AddScoped(serv => new PizzaConnector(serv, name, baseUri));
        services.AddScoped(serv => new PizzaPocotaContext(serv, name));
        services.AddScoped<PizzaJsonConverterFactory>();
        services.AddKeyedScoped<PocotaContext>(name, (serv, key) => serv.GetRequiredService<PizzaPocotaContext>());
        services.AddKeyedScoped<Connector>(name, (serv, key) => serv.GetRequiredService<PizzaConnector>());
        services.AddTransient<PizzaJsonConverter>();
        services.AddTransient<SauceJsonConverter>();
        services.AddTransient<ToppingJsonConverter>();
        return services;
    }
}
