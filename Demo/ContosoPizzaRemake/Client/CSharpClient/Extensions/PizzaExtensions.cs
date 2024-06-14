/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaExtensions                     //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-14T23:31:32.                                 //
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
        string servicesKey = "Pizza",
        Uri? baseUri = null
    )
    {
        services.AddScoped(serv => new PizzaConnector(serv, servicesKey, baseUri));
        services.AddScoped(serv => new PizzaPocotaContext(serv, servicesKey));
        services.AddScoped<PizzaJsonConverterFactory>();
        services.AddKeyedScoped<PocotaContext>(servicesKey, (serv, key) => serv.GetRequiredService<PizzaPocotaContext>());
        services.AddKeyedScoped<Connector>(servicesKey, (serv, key) => serv.GetRequiredService<PizzaConnector>());
        services.AddTransient<PizzaJsonConverter>();
        services.AddTransient<SauceJsonConverter>();
        services.AddTransient<ToppingJsonConverter>();
        return services;
    }
}
