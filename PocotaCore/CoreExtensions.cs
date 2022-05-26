using Microsoft.Extensions.DependencyInjection;

namespace Net.Leksi.Pocota.Core;

public static class CoreExtensions
{
    public static IServiceCollection AddPocotaCore(this IServiceCollection services, Action<IServiceCollection> configure)
    {
        Manager.AddPocotaCore(services, configure);
        return services;
    }

    public static IServiceCollection AddKeyMapping<TSource>(this IServiceCollection services, IDictionary<string, Type> fieldDefinitions)
        where TSource: class
    {
        if (services is Manager instance)
        {
            instance.AddKeyMapping(typeof(TSource), fieldDefinitions);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddKeyMapping)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }

    public static IServiceCollection AddKeyMapping(this IServiceCollection services, Type sourceType, IDictionary<string, Type> fieldDefinitions)
    {
        if (services is Manager instance)
        {
            instance.AddKeyMapping(sourceType, fieldDefinitions);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddKeyMapping)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }

    public static IServiceCollection AddKeyMapping(this IServiceCollection services, Type sourceType, Type exampleType)
    {
        if (services is Manager instance)
        {
            instance.AddKeyMapping(sourceType, exampleType);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddKeyMapping)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }

    public static IServiceCollection AddKeyMapping<TSource>(this IServiceCollection services, Type exampleType)
        where TSource : class
    {
        if (services is Manager instance)
        {
            instance.AddKeyMapping(typeof(TSource), exampleType);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddKeyMapping)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }

    public static IServiceCollection AddKeyMapping<TSource, TExample>(this IServiceCollection services)
        where TSource : class
        where TExample : class
    {
        if (services is Manager instance)
        {
            instance.AddKeyMapping(typeof(TSource), typeof(TExample));
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddKeyMapping)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }


}
