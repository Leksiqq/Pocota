/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaExtensions                            //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-13T13:50:47.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Server;

namespace ContosoPizza;


public static class PizzaExtensions
{
    public static IServiceCollection AddPizza(
        this IServiceCollection services, 
        Type serviceImplementation,
        Action<IServiceCollection>? additionalConfig = null
    )
    {
        services.AddScoped<PocotaContext>();
        services.AddScoped<PizzaJsonConverterFactory>();
        services.AddScoped(typeof(PizzaServiceBase), serviceImplementation);
        services.AddTransient<PizzaJsonConverter>();
        services.AddTransient<PizzaPocota>();
        services.AddTransient<SauceJsonConverter>();
        services.AddTransient<SaucePocota>();
        services.AddTransient<ToppingJsonConverter>();
        services.AddTransient<ToppingPocota>();
        additionalConfig?.Invoke(services);
        return services;
    }
}
