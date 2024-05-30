/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaExtensions                     //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-30T18:11:42.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models.Client;
using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client;

namespace ContosoPizza.Client;


public static class PizzaExtensions
{
    public static IServiceCollection AddPizza(
        this IServiceCollection services
    )
    {
        services.AddScoped<PizzaConnector>();
        services.AddScoped<PizzaPocotaContext>();
        services.AddScoped<PizzaJsonConverterFactory>();
        services.AddKeyedScoped<PocotaContext>("Pizza", (serv, key) => serv.GetRequiredService<PizzaPocotaContext>());
        services.AddKeyedScoped<Connector>("Pizza", (serv, key) => serv.GetRequiredService<PizzaConnector>());
        services.AddTransient<PizzaJsonConverter>();
        services.AddTransient<SauceJsonConverter>();
        services.AddTransient<ToppingJsonConverter>();
        PizzaPocotaContext.Touch();

        return services;
    }
}
