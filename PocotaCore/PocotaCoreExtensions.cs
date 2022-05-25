using Microsoft.Extensions.DependencyInjection;

namespace Net.Leksi.Pocota.Core;

public static class PocotaCoreExtensions
{
    public static IServiceCollection AddPocotaCore(this IServiceCollection services, Action<IServiceCollection> configure)
    {
        PocotaManager.AddPocotaCore(services, configure);
        return services;
    }

    public static IServiceCollection AddKeyMapping<TSource>(this IServiceCollection services, IEnumerable<string> fieldNames)
        where TSource: class
    {
        if (services is PocotaManager instance)
        {
            instance.AddKeyMapping(typeof(TSource), fieldNames);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddKeyMapping)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }

    public static IServiceCollection AddKeyMapping(this IServiceCollection services, Type sourceType, IEnumerable<string> fieldNames)
    {
        if (services is PocotaManager instance)
        {
            instance.AddKeyMapping(sourceType, fieldNames);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddKeyMapping)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }

    public static IServiceCollection AddKeyMapping(this IServiceCollection services, Type sourceType, Type exampleType)
    {
        if (services is PocotaManager instance)
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
        if (services is PocotaManager instance)
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
        if (services is PocotaManager instance)
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
