using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Net.Leksi.Pocota.Core;

/// <summary>
/// <para xml:lang="ru">
/// Класс, предоставляющий расширения для <see cref="IServiceCollection"/> предназначенное для 
/// включения инфраструктуры ядра фреймворка Pocota.
/// </para>
/// <para xml:lang="en">
/// Class providing extensions for <see cref="IServiceCollection"/> intended for
/// enabling Pocota framework core infrastructure.
/// </para>
/// </summary>
public static class CoreExtensions
{
    /// <summary>
    /// <para xml:lang="ru">
    /// Включения инфраструктуры ядра фреймворка Pocota.
    /// Регистрирует интерфейсы для модельных POCO-классов. 
    /// Определяет первичные ключи для объектов модельных POCO-классов. 
    /// </para>
    /// <para xml:lang="en">
    /// Enable Pocota framework core infrastructure.
    /// Registers interfaces for model POCO classes.
    /// Defines primary keys for objects of model POCO classes.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// <para xml:lang="ru">
    /// Коллекция служб.
    /// </para>
    /// <para xml:lang="en">
    /// Collection of services.
    /// </para>
    /// </param>
    /// <param name="configure">
    /// <para xml:lang="ru">
    /// <see cref="Action{IServiceCollection}"/> для выполнения регистрации и определения первичных ключей.
    /// </para>
    /// <para xml:lang="en">
    /// <see cref="Action{IServiceCollection}"/> to perform registration and define primary keys.
    /// </para>
    /// </param>
    /// <example>
    /// <code>
    /// IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
    ///.AddDtoCore(services =>
    ///    {
    ///        services.AddTransient&lt;IShipCall, ShipCall&gt;();
    ///        services.AddTransient&lt;IShipCallForListing, ShipCall&gt;();
    ///        services.AddTransient&lt;IShipCallAdditionalInfo, ShipCall&gt;();
    ///        services.AddTransient&lt;IArrivalShipCall, ShipCall&gt;();
    ///        services.AddTransient&lt;IDepartureShipCall, ShipCall&gt;();
    ///        
    ///        services.AddKeyMapping&lt;ShipCall&gt;(new Dictionary&lt;string, Type&gt; { { "ID_LINE", typeof(string) }, { "ID_ROUTE", typeof(int) } });
    ///        
    ///        services.AddTransient&lt;ILocation, Location&gt;();
    ///        
    ///        services.AddKeyMapping&lt;Location&gt;(new Dictionary&lt;string, Type&gt; { { "ID_LOCATION", typeof(string) } });
    ///        
    ///        services.AddTransient&lt;IRoute, Route&gt;();
    ///        services.AddTransient&lt;IRouteShort, Route&gt;();
    ///        
    ///        services.AddKeyMapping&lt;Route&gt;(new Dictionary&lt;string, Type&gt; { { "ID_LINE", typeof(string) }, { "ID_RHEAD", typeof(int) } });
    ///        
    ///        services.AddTransient&lt;ILine, Line&gt;();
    ///        
    ///        services.AddKeyMapping&lt;Line&gt;(new Dictionary&lt;string, Type&gt; { { "ID_LINE", typeof(string) } });
    ///        
    ///        services.AddTransient&lt;IVessel, Vessel&gt;();
    ///        services.AddTransient&lt;IVesselShort, Vessel&gt;();
    ///        
    ///        services.AddKeyMapping&lt;Vessel&gt;(new Dictionary&lt;string, Type&gt; { { "ID_VESSEL", typeof(string) } });
    ///        
    ///        services.AddTransient&lt;ITravelForListing, Travel&gt;();
    ///    });
    ///host = hostBuilder.Build();
    /// </code>
    /// </example>
    public static IServiceCollection AddPocotaCore(this IServiceCollection services, Action<IServiceCollection> configure)
    {
        Container.AddPocotaCore(services, configure);
        return services;
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Определяет структуру первичного ключа для типа <с>targetType</с>.
    /// </para>
    /// <para xml:lang="en">
    /// Defines the primary key structure for the type <с>targetType</с>.
    /// </para>
    /// </summary>
    /// <typeparam name="Target">
    /// <para xml:lang="ru">
    /// Тип, для которого определяется структура первичного ключа.
    /// </para>
    /// <para xml:lang="en">
    /// The type for which the primary key structure is defined.
    /// </para>
    /// </typeparam>
    /// <param name="services">
    /// <para xml:lang="ru">
    /// Коллекция служб.
    /// </para>
    /// <para xml:lang="en">
    /// Collection of services.
    /// </para>
    /// </param>
    /// <param name="fieldDefinitions">
    /// <para xml:lang="ru">
    /// Словарь, определяющий структуру первичного ключа через пары "имя-тип".
    /// </para>
    /// <para xml:lang="en">
    /// A dictionary that defines the primary key structure in terms of name-type pairs.
    /// </para>
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// <para xml:lang="ru">
    /// Выбрасывается при вызове вне делегата конфигурирования или когда конфигурирование уже завершено или первичный ключ для целевого типа уже определён.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown when called outside the configuration delegate, or when configuration is already complete or a primary key for the target type is already defined.
    /// </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para xml:lang="ru">
    /// Выбрасывается в случаях, когда целевой тип не является классом.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown when the target type is not a class.
    /// </para>
    /// </exception>
    public static IServiceCollection AddPrimaryKey<Target>(this IServiceCollection services, IDictionary<string, Type> fieldDefinitions)
        where Target: class
    {
        if (services is Container instance)
        {
            instance.AddPrimaryKey(typeof(Target), fieldDefinitions);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddPrimaryKey)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Определяет структуру первичного ключа для типа <с>targetType</с>.
    /// </para>
    /// <para xml:lang="en">
    /// Defines the primary key structure for the type <с>targetType</с>.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// <para xml:lang="ru">
    /// Коллекция служб.
    /// </para>
    /// <para xml:lang="en">
    /// Collection of services.
    /// </para>
    /// </param>
    /// <param name="targetType">
    /// <para xml:lang="ru">
    /// Тип, для которого определяется структура первичного ключа.
    /// </para>
    /// <para xml:lang="en">
    /// The type for which the primary key structure is defined.
    /// </para>
    /// </param>
    /// <param name="fieldDefinitions">
    /// <para xml:lang="ru">
    /// Словарь, определяющий структуру первичного ключа через пары "имя-тип".
    /// </para>
    /// <para xml:lang="en">
    /// A dictionary that defines the primary key structure in terms of name-type pairs.
    /// </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// <para xml:lang="ru">
    /// Выбрасывается при вызове вне делегата конфигурирования или когда конфигурирование уже завершено или первичный ключ для целевого типа уже определён.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown when called outside the configuration delegate, or when configuration is already complete or a primary key for the target type is already defined.
    /// </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para xml:lang="ru">
    /// Выбрасывается в случаях, когда целевой тип не является классом.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown when the target type is not a class.
    /// </para>
    /// </exception>
    public static IServiceCollection AddPrimaryKey(this IServiceCollection services, Type targetType, IDictionary<string, Type> fieldDefinitions)
    {
        if (services is Container instance)
        {
            instance.AddPrimaryKey(targetType, fieldDefinitions);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddPrimaryKey)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Определяет структуру первичного ключа для типа <с>targetType</с>.
    /// </para>
    /// <para xml:lang="en">
    /// Defines the primary key structure for the type <с>targetType</с>.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// <para xml:lang="ru">
    /// Коллекция служб.
    /// </para>
    /// <para xml:lang="en">
    /// Collection of services.
    /// </para>
    /// </param>
    /// <param name="targetType">
    /// <para xml:lang="ru">
    /// Тип, для которого определяется структура первичного ключа.
    /// </para>
    /// <para xml:lang="en">
    /// The type for which the primary key structure is defined.
    /// </para>
    /// </param>
    /// <param name="exampleType">
    /// <para xml:lang="ru">
    /// Тип, подающий пример определения первичного ключа. В целевом типе ключ такой же, как этом типе.
    /// </para>
    /// <para xml:lang="en">
    /// A type that exemplifies the definition of a primary key. In the target type, the key is the same as this type.
    /// </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// <para xml:lang="ru">
    /// Выбрасывается при вызове вне делегата конфигурирования или когда конфигурирование уже завершено или первичный ключ для целевого типа уже определён.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown when called outside the configuration delegate, or when configuration is already complete or a primary key for the target type is already defined.
    /// </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para xml:lang="ru">
    /// Выбрасывается в случаях, когда целевой тип не является классом.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown when the target type is not a class.
    /// </para>
    /// </exception>
    public static IServiceCollection AddPrimaryKey(this IServiceCollection services, Type targetType, Type exampleType)
    {
        if (services is Container instance)
        {
            instance.AddPrimaryKey(targetType, exampleType);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddPrimaryKey)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Определяет структуру первичного ключа для типа <с>targetType</с>.
    /// </para>
    /// <para xml:lang="en">
    /// Defines the primary key structure for the type <с>targetType</с>.
    /// </para>
    /// </summary>
    /// <typeparam name="Target">
    /// <para xml:lang="ru">
    /// Тип, для которого определяется структура первичного ключа.
    /// </para>
    /// <para xml:lang="en">
    /// The type for which the primary key structure is defined.
    /// </para>
    /// </typeparam>
    /// <param name="services">
    /// <para xml:lang="ru">
    /// Коллекция служб.
    /// </para>
    /// <para xml:lang="en">
    /// Collection of services.
    /// </para>
    /// </param>
    /// <param name="exampleType">
    /// <para xml:lang="ru">
    /// Тип, подающий пример определения первичного ключа. В целевом типе ключ такой же, как этом типе.
    /// </para>
    /// <para xml:lang="en">
    /// A type that exemplifies the definition of a primary key. In the target type, the key is the same as this type.
    /// </para>
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// <para xml:lang="ru">
    /// Выбрасывается при вызове вне делегата конфигурирования или когда конфигурирование уже завершено или первичный ключ для целевого типа уже определён.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown when called outside the configuration delegate, or when configuration is already complete or a primary key for the target type is already defined.
    /// </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para xml:lang="ru">
    /// Выбрасывается в случаях, когда целевой тип не является классом.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown when the target type is not a class.
    /// </para>
    /// </exception>
    public static IServiceCollection AddPrimaryKey<Target>(this IServiceCollection services, Type exampleType)
        where Target : class
    {
        if (services is Container instance)
        {
            instance.AddPrimaryKey(typeof(Target), exampleType);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddPrimaryKey)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Определяет структуру первичного ключа для типа <с>targetType</с>.
    /// </para>
    /// <para xml:lang="en">
    /// Defines the primary key structure for the type <с>targetType</с>.
    /// </para>
    /// </summary>
    /// <typeparam name="Target">
    /// <para xml:lang="ru">
    /// Тип, для которого определяется структура первичного ключа.
    /// </para>
    /// <para xml:lang="en">
    /// The type for which the primary key structure is defined.
    /// </para>
    /// </typeparam>
    /// <typeparam name="Example">
    /// <para xml:lang="ru">
    /// Тип, подающий пример определения первичного ключа. В целевом типе ключ такой же, как этом типе.
    /// </para>
    /// <para xml:lang="en">
    /// A type that exemplifies the definition of a primary key. In the target type, the key is the same as this type.
    /// </para>
    /// </typeparam>
    /// <param name="services">
    /// <para xml:lang="ru">
    /// Коллекция служб.
    /// </para>
    /// <para xml:lang="en">
    /// Collection of services.
    /// </para>
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// <para xml:lang="ru">
    /// Выбрасывается при вызове вне делегата конфигурирования или когда конфигурирование уже завершено или первичный ключ для целевого типа уже определён.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown when called outside the configuration delegate, or when configuration is already complete or a primary key for the target type is already defined.
    /// </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para xml:lang="ru">
    /// Выбрасывается в случаях, когда целевой тип не является классом.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown when the target type is not a class.
    /// </para>
    /// </exception>
    public static IServiceCollection AddPrimaryKey<Target, Example>(this IServiceCollection services)
        where Target : class
        where Example : class
    {
        if (services is Container instance)
        {
            instance.AddPrimaryKey(typeof(Target), typeof(Example));
        }
        else
        {
            throw new InvalidOperationException($"{nameof(AddPrimaryKey)} can be called only inside {nameof(AddPocotaCore)} configure action");
        }
        return services;
    }


}
