/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaExtensions                     //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-31T16:57:58.                                 //
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
        string servicesKey = "Pizza"
    )
    {
        services.AddScoped(serv => new PizzaConnector(serv, servicesKey));
        services.AddScoped(serv => new PizzaPocotaContext(serv, servicesKey));
        services.AddScoped<PizzaJsonConverterFactory>();
        services.AddKeyedScoped<PocotaContext>(servicesKey, (serv, key) => serv.GetRequiredService<PizzaPocotaContext>());
        services.AddKeyedScoped<Connector>(servicesKey, (serv, key) => serv.GetRequiredService<PizzaConnector>());
        services.AddTransient<PizzaJsonConverter>();
        services.AddTransient<SauceJsonConverter>();
        services.AddTransient<ToppingJsonConverter>();
        PizzaPocotaContext.Touch();

        return services;
    }
}
