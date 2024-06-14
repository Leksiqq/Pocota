/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaExtensions                            //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-14T17:14:56.                                 //
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
        services.AddHttpContextAccessor();
        services.AddScoped<PocotaContext>();
        services.AddScoped<PizzaJsonConverterFactory>();
        services.AddScoped(typeof(PizzaServiceBase), serviceImplementation);
        services.AddTransient<PizzaJsonConverter>();
        services.AddTransient<PizzaPocotaEntity>();
        PizzaController.Entities.Add(typeof(Pizza));
        services.AddTransient<SauceJsonConverter>();
        services.AddTransient<SaucePocotaEntity>();
        PizzaController.Entities.Add(typeof(Sauce));
        services.AddTransient<ToppingJsonConverter>();
        services.AddTransient<ToppingPocotaEntity>();
        PizzaController.Entities.Add(typeof(Topping));

        additionalConfig?.Invoke(services);

        ServiceDescriptor probe;

        probe = new ServiceDescriptor(typeof(IAccessCalculator), typeof(Pizza), typeof(PizzaAccessBase), ServiceLifetime.Scoped);
        if(!services.Contains(probe, AccessCalculatorServicesEqualityComparer.Instance))
        {
            services.AddKeyedScoped<IAccessCalculator>(typeof(Pizza), (s, k) => new PizzaAccessBase(s));
        }
        else
        {
            ServiceDescriptor sd = services.Where(s => AccessCalculatorServicesEqualityComparer.Instance.Equals(probe, s)).First();
            if (sd.Lifetime is not ServiceLifetime.Scoped)
            {
                throw new InvalidOperationException($"{nameof(IAccessCalculator)} service expected to be scoped, got: {sd}.");
            }
        }
        probe = new ServiceDescriptor(typeof(IAccessCalculator), typeof(Sauce), typeof(SauceAccessBase), ServiceLifetime.Scoped);
        if(!services.Contains(probe, AccessCalculatorServicesEqualityComparer.Instance))
        {
            services.AddKeyedScoped<IAccessCalculator>(typeof(Sauce), (s, k) => new SauceAccessBase(s));
        }
        else
        {
            ServiceDescriptor sd = services.Where(s => AccessCalculatorServicesEqualityComparer.Instance.Equals(probe, s)).First();
            if (sd.Lifetime is not ServiceLifetime.Scoped)
            {
                throw new InvalidOperationException($"{nameof(IAccessCalculator)} service expected to be scoped, got: {sd}.");
            }
        }
        probe = new ServiceDescriptor(typeof(IAccessCalculator), typeof(Topping), typeof(ToppingAccessBase), ServiceLifetime.Scoped);
        if(!services.Contains(probe, AccessCalculatorServicesEqualityComparer.Instance))
        {
            services.AddKeyedScoped<IAccessCalculator>(typeof(Topping), (s, k) => new ToppingAccessBase(s));
        }
        else
        {
            ServiceDescriptor sd = services.Where(s => AccessCalculatorServicesEqualityComparer.Instance.Equals(probe, s)).First();
            if (sd.Lifetime is not ServiceLifetime.Scoped)
            {
                throw new InvalidOperationException($"{nameof(IAccessCalculator)} service expected to be scoped, got: {sd}.");
            }
        }
       foreach (var service in services)
        {
            if (typeof(PizzaDbContext).IsAssignableFrom(service.ServiceType))
            {
                ServiceDescriptor sd = new(typeof(PizzaDbContext), service.ServiceType, service.Lifetime);
                services.Add(sd);
                break;
            }
        }
        return services;
    }
}
