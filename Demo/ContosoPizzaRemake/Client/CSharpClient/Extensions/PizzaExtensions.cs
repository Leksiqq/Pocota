/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaExtensions                     //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-04T18:29:40.                                 //
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
        services.AddScoped<PocotaContext>(serv => serv.GetRequiredService<PizzaPocotaContext>());
        services.AddScoped<Connector>(serv => serv.GetRequiredService<PizzaConnector>());
        services.AddTransient<PizzaJsonConverter>();
        services.AddTransient<SauceJsonConverter>();
        services.AddTransient<ToppingJsonConverter>();

        return services;
    }
}
