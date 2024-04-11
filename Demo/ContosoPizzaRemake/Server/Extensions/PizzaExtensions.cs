/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaExtensions                            //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-11T18:57:55.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoPizza;


public static class PizzaExtensions
{
    public static IServiceCollection AddPizza(
        this IServiceCollection services, 
        Type serviceImplementation,
        Action<IServiceCollection>? additionalConfig = null
    )
    {
        services.AddScoped<PizzaJsonConverterFactory>();
        services.AddScoped(typeof(PizzaServiceBase), serviceImplementation);
        services.AddTransient<PizzaJsonConverter>();
        services.AddTransient<SauceJsonConverter>();
        services.AddTransient<ToppingJsonConverter>();
        additionalConfig?.Invoke(services);
        return services;
    }
}
