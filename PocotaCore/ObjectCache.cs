using Microsoft.Extensions.DependencyInjection;

namespace Net.Leksi.Pocota.Core;

/// <summary>
/// <para xml:lang="ru">
/// Класс объектов для хранения и доступа к уже созданным при загрузке или десериализации объектам
/// для предотвращения их дублирования
/// </para>
/// <para xml:lang="en">
/// An object class for storing and accessing objects already created during loading or deserialization
/// to prevent duplication
/// </para>
/// </summary>
public class ObjectCache
{

    private static readonly KeyEqualityComparer _keyComparer = new();
    private readonly TypesForest _typesForest;
    private readonly Container _manager;

    private Dictionary<Type, Dictionary<object[], object>> _objectsCache = new();

    /// <summary>
    /// <para xml:lang="ru">
    /// Количество объектов в кеше
    /// </para>
    /// <para xml:lang="en">
    /// Number of objects in the cache
    /// </para>
    /// </summary>
    public int Count => _objectsCache.Count;

    /// <summary>
    /// <para xml:lang="ru">
    /// Конструктор
    /// </para>
    /// <para xml:lang="en">
    /// Constructor
    /// </para>
    /// </summary>
    /// <param name="typesForest"></param>
    public ObjectCache(IServiceProvider serviceProvider) =>
        (_typesForest, _manager) = (serviceProvider.GetRequiredService<TypesForest>(), serviceProvider.GetRequiredService<Container>());


    /// <summary>
    /// <para xml:lang="ru">
    /// Пытается извлечь из кеша ссылку на объект указанного типа и с указанным набором ключевых свойств
    /// </para>
    /// <para xml:lang="en">
    /// Attempts to retrieve from the cache a reference to an object of the specified type and with the specified set of key properties
    /// </para>
    /// </summary>
    /// <param name="type">
    /// <para xml:lang="ru">
    /// Интерфейсный тип, под которым искомый объект помёщён в кеш
    /// </para>
    /// <para xml:lang="en">
    /// Interface type under which the searched object is placed in the cache
    /// </para>
    /// </param>
    /// <param name="keyRing">
    /// <para xml:lang="ru">
    /// Набор значений ключевых свойств искомого объекта
    /// </para>
    /// <para xml:lang="en">
    /// Set of values of the key properties of the searched object
    /// </para>
    /// </param>
    /// <param name="result">
    /// <para xml:lang="ru">
    /// Найденный объект в случае успеха
    /// </para>
    /// <para xml:lang="en">
    /// Found object if successful
    /// </para>
    /// </param>
    /// <returns>
    /// <para xml:lang="ru">
    /// Успешно или нет
    /// </para>
    /// <para xml:lang="en">
    /// Successful or not
    /// </para>
    /// </returns>
    public bool TryGet(Type type, object source, out object? result)
    {
        ThrowIfIsNull(type);
        ThrowIfNotRegistered(type);
        if (_objectsCache.ContainsKey(type))
        {
            KeyRing? keyRing = _manager.GetKeyRing(source);
            if(keyRing is { })
            {
                return _objectsCache[type].TryGetValue(keyRing.PrimaryKey, out result);
            }
        }
        result = default;
        return false;
    }

    /// <summary>
    /// <para xml:lang="ru">
    /// Добавляет в кеш или заменяет существующий объект как объект определённого типа.
    /// </para>
    /// <para xml:lang="en">
    /// Adds to the cache or replaces an existing object as an object of a certain type.
    /// </para>    
    /// </summary>
    /// <param name="type">
    /// <para xml:lang="ru">
    /// Интерфейсный тип, под которым объект помещается в кеш
    /// </para>
    /// <para xml:lang="en">
    /// Interface type under which the object is placed in the cache
    /// </para>
    /// </param>
    /// <param name="value">
    /// <para xml:lang="ru">
    /// Помещаемый в кеш объект
    /// </para>
    /// <para xml:lang="en">
    /// The object to be cached
    /// </para>
    /// </param>
    /// <returns>
    /// <para xml:lang="ru">
    /// <c>true</c>, если объект помещён в кэш, <c>false</c>, если найден.
    /// </para>
    /// <para xml:lang="en">
    /// <c>true</c> if the object is cached, <c>false</c> if found.
    /// </para>
    /// </returns>
    public bool Add(Type type, object value)
    {
        ThrowIfIsNull(value);
        ThrowIfIsNull(type);
        ThrowIfNotRegistered(type);
        if (!type.IsAssignableFrom(value.GetType()))
        {
            throw new InvalidCastException($"{nameof(value)} must be {type}");
        }
        KeyRing? keyRing = _manager.GetKeyRing(value);
        if (keyRing is null)
        {
            throw new ArgumentException($"{nameof(value)} must be of type with defined primary key");
        }
        if (!keyRing.IsAssigned)
        {
            throw new ArgumentException($"{nameof(value)} must have assigned primary key");
        }
        bool result = true;
        if (!_objectsCache.ContainsKey(type))
        {
            _objectsCache.Add(type, new Dictionary<object[], object>(_keyComparer));
            if (!_objectsCache.ContainsKey(value.GetType()))
            {
                _objectsCache[value.GetType()] = new Dictionary<object[], object>(_keyComparer);
            }
        }
        if (!_objectsCache[type].ContainsKey(keyRing.PrimaryKey))
        {
            if (_objectsCache[value.GetType()].ContainsKey(keyRing.PrimaryKey))
            {
                _typesForest.Inject(type, value, _objectsCache[value.GetType()][keyRing.PrimaryKey]);
                _objectsCache[type][keyRing.PrimaryKey] = _objectsCache[value.GetType()][keyRing.PrimaryKey];
                result = false;
            }
            else
            {
                _objectsCache[value.GetType()][keyRing.PrimaryKey] = value;
                _objectsCache[type][keyRing.PrimaryKey] = value;
            }
        }
        return result;
    }

    /// <summary>
    /// <para xml:lang="ru">
    /// Очищает кеш
    /// </para>
    /// <para xml:lang="en">
    /// Clear cache
    /// </para>
    /// </summary>
    public void Clear()
    {
        _objectsCache.Clear();
    }

    private void ThrowIfNotRegistered(Type type)
    {
        if (!_manager.ContainsServiceType(type))
        {
            throw new ArgumentException($"{nameof(type)} must be registered at Pocota container");
        }
    }

    private static void ThrowIfIsNull(object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }
    }

}
