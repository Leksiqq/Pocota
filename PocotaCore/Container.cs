using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Net.Leksi.Pocota.Core;
/// <summary>
/// <para xml:lang="ru">
/// Класс объекта для регистрации множества интерфейсов, реализуемых POCO-типами, объекты которых будут загружаться, 
/// сериализоваться и десериализоваться в зависимости от применяемого интерфейса. Поддерживает первичные ключи объектов.
/// </para>
/// <para xml:lang="en">
/// Object class for registering a set of interfaces implemented by POCO types whose objects will be loaded,
/// serialize and deserialize depending on the applied interface. Supports primary keys of objects.
/// </para>
/// </summary>
public class Container : IServiceCollection
{
    private readonly Dictionary<Type, Type> _registered = new();
    private readonly Dictionary<Type, Dictionary<string, KeyDefinition>> _keyMap = new();
    private readonly Dictionary<Type, Type> _exampleKeyMap = new();
    private readonly ConditionalWeakTable<object, KeyRing> _attachedKeys = new();
    private readonly ConcurrentDictionary<Type, Type?> _mappedTypesCache = new();

    internal IServiceProvider? ServiceProvider { get; set; } = null;
    internal IServiceCollection? ServiceDescriptors { get; set; } = null;

    private Container() { }

    /// <summary>
    /// <para xml:lang="ru">
    /// Сообщает, были ли зарегистрированы первичные ключи.
    /// </para>
    /// <para xml:lang="en">
    /// Tells if primary keys have been registered.
    /// </para>
    /// </summary>
    public bool HasMappedPrimaryKeys => _keyMap.Count > 0;
    /// <summary>
    /// <para xml:lang="ru">
    /// Проверяет зарегистрирован ли интерфейс
    /// </para>
    /// <para xml:lang="en">
    /// Checks if the interface is registered
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// <para xml:lang="ru">
    /// Проверяемый интерфейс
    /// </para>
    /// <para xml:lang="en">
    /// Tested interface
    /// </para>
    /// </typeparam>
    /// <returns></returns>
    public bool IsRegistered<T>() where T : class
    {
        return ContainsServiceType(typeof(T));
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Проверяет зарегистрирован ли интерфейс
    /// </para>
    /// <para xml:lang="en">
    /// Checks if the interface is registered
    /// </para>
    /// </summary>
    /// <param name="type">
    /// <para xml:lang="ru">
    /// Проверяемый интерфейс
    /// </para>
    /// <para xml:lang="en">
    /// Tested interface
    /// </para>
    /// </param>
    /// <returns></returns>
    public bool ContainsServiceType(Type type)
    {
        return _registered.ContainsKey(type);
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Возвращает настоящий тип зарегистрированного интерфейса
    /// </para>
    /// <para xml:lang="en">
    /// Returns the actual type of the registered interface
    /// </para>
    /// </summary>
    /// <param name="serviceType">
    /// <para xml:lang="ru">
    /// Проверяемый интерфейс
    /// </para>
    /// <para xml:lang="en">
    /// Tested interface
    /// </para>
    /// </param>
    /// <returns>
    /// <para xml:lang="ru">
    /// Настоящий тип для зарегистрированного интерфейса или <c>null</c> для незарегистрированного
    /// </para>
    /// </returns>
    public Type? GetActualType(Type serviceType)
    {
        return _registered.ContainsKey(serviceType) ? _registered[serviceType] : null;
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Возвращает аксессор к первичному ключу указанного объекта.
    /// </para>
    /// <para xml:lang="en">
    /// Returns an accessor to the primary key of the specified object.
    /// </para>
    /// </summary>
    /// <param name="source">
    /// <para xml:lang="ru">
    /// Объект, доступ к первичному ключу которого нужно получить.
    /// </para>
    /// <para xml:lang="en">
    /// The object whose primary key needs to be accessed.
    /// </para>
    /// </param>
    /// <returns>
    /// <para xml:lang="ru">
    /// Аксессор <see cref="KeyRing"/> к первичному ключу указанного объекта.
    /// </para>
    /// <para xml:lang="en">
    /// Accessor <see cref="KeyRing"/> to the primary key of the specified object.
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <para xml:lang="ru">
    /// Параметр <с>source</с> не может быть <c>null</c>
    /// </para>
    /// <para xml:lang="en">
    /// Parameter <с>source</с> cannot be <c>null</c>
    /// </para>
    /// </exception>
    public KeyRing? GetKeyRing(object source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        KeyRing? keyRing = null;
        if (_attachedKeys.TryGetValue(source, out keyRing))
        {
            return keyRing;
        }
        return CreateKeyRing(source);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <para xml:lang="ru">
    /// Выбрасывается в случаях, когда конфигурирование уже завершено или добавляется служба со временем жизни, отличным от <see cref="ServiceLifetime.Transient"/>.
    /// </para>
    /// <para xml:lang="en">
    /// Thrown in cases where configuration has already been completed or a service with a lifetime other than <see cref="ServiceLifetime.Transient"/> is being added.
    /// </para>
    /// </exception>
    public void Add(ServiceDescriptor item)
    {
        ThrowIfConfigured();
        ThrowIfNotTransient(item);
        ServiceDescriptor implementationDescriptor = null!;
        if (item.ImplementationType is { })
        {
            _registered[item.ServiceType] = item.ImplementationType;
            implementationDescriptor = new ServiceDescriptor(_registered[item.ServiceType], item.ImplementationType, item.Lifetime);
        }
        else if(item.ImplementationFactory is { })
        {
            _registered[item.ServiceType] = item.ImplementationFactory.Method.ReturnType;
            implementationDescriptor = new ServiceDescriptor(_registered[item.ServiceType], item.ImplementationFactory, item.Lifetime);
        }
        ServiceDescriptors!.Add(item);
        if (!ServiceDescriptors.Contains(implementationDescriptor))
        {
            _registered[_registered[item.ServiceType]] = _registered[item.ServiceType];
           ServiceDescriptors!.Add(implementationDescriptor);
        }
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Возвращает определение первичного ключа для указанного типа.
    /// </para>
    /// <para xml:lang="en">
    /// Returns the primary key definition for the specified type.
    /// </para>
    /// </summary>
    /// <param name="type">
    /// <para xml:lang="ru">
    /// Указанный тип.
    /// </para>
    /// <para xml:lang="en">
    /// Specified type.
    /// </para>
    /// </param>
    /// <returns>
    /// <para xml:lang="ru">
    /// Словарь, определяющий структуру первичного ключа через пары "имя-тип".
    /// </para>
    /// <para xml:lang="en">
    /// A dictionary that defines the primary key structure in terms of name-type pairs.
    /// </para>
    /// </returns>
    public Dictionary<string, Type>? GetPrimaryKeyDefinition(Type type)
    {
        Type? mapped = GetMappedType(type);
        if (mapped is { })
        {
            return _keyMap[mapped].ToDictionary(v => v.Key, v => v.Value.Type);
        }
        return null;
    }

    #region Unused
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public int Count => throw new NotImplementedException();
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public bool IsReadOnly => throw new NotImplementedException();
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public ServiceDescriptor this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public int IndexOf(ServiceDescriptor item)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public void Insert(int index, ServiceDescriptor item)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public void Clear()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public bool Contains(ServiceDescriptor item)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public bool Remove(ServiceDescriptor item)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Не используется.
    /// </para>
    /// <para xml:lang="en">
    /// Not used.
    /// </para>
    /// <inheritdoc/>
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
    #endregion Unused

    private Type? GetMappedType(Type actualType)
    {
        Type? mapped = null;
        if (!_mappedTypesCache.TryGetValue(actualType, out mapped))
        {
            Type? current = actualType;
            while (current is { } && !_keyMap.ContainsKey(current))
            {
                current = current!.BaseType;
            }
            mapped = current;
            _mappedTypesCache.TryAdd(actualType, mapped);
        }
        return mapped;
    }

    internal KeyRing? CreateKeyRing(object source)
    {
        KeyRing? keyRing = null;
        if(!_attachedKeys.TryGetValue(source, out keyRing))
        {
            Type? mapped = GetMappedType(source.GetType());
            if (mapped is { })
            {
                keyRing = new KeyRing(this, _keyMap[mapped]);
                keyRing.PrimaryKey = new object[_keyMap[mapped].Count];
                keyRing.Source = source;
                _attachedKeys.Add(source, keyRing);
            }
        }
        return keyRing;
    }

    internal void AddPrimaryKey(Type targetType, IDictionary<string, Type> fieldDefinitions)
    {
        ThrowIfConfigured();
        ThrowIfNotClass(nameof(targetType), targetType);
        ThrowIfAlreadyMapped(targetType);
        _keyMap[targetType] = new Dictionary<string, KeyDefinition>();
        int pos = 0;
        foreach (string name in fieldDefinitions.Keys.OrderBy(v => v))
        {
            _keyMap[targetType][name] = new KeyDefinition { Index = pos, Type = fieldDefinitions[name] };
            ++pos;
        }
    }

    internal void AddPrimaryKey(Type targetType, Type example)
    {
        ThrowIfConfigured();
        ThrowIfNotClass(nameof(targetType), targetType);
        ThrowIfNotClass(nameof(example), example);
        ThrowIfAlreadyMapped(targetType);
        _exampleKeyMap[targetType] = example;
    }

    internal static void AddPocotaCore(IServiceCollection services, Action<IServiceCollection> configure)
    {
        Container instance = new();
        services.AddSingleton<Container>(serviceProvider =>
        {
            instance.ServiceProvider = serviceProvider;
            return instance;
        });
        services.AddSingleton<TypesForest>();
        services.AddTransient<ObjectCache>();
        instance.ServiceDescriptors = services;
        configure?.Invoke(instance);
        instance.MapExamples();
        instance.CheckKeyMappings();
        instance.ServiceDescriptors = null;


    }

    private static void ThrowIfNotTransient(ServiceDescriptor item)
    {
        if (item.Lifetime is not ServiceLifetime.Transient)
        {
            throw new InvalidOperationException($"{item.ServiceType} must be added as {ServiceLifetime.Transient}, but is added as {item.Lifetime}");
        }
    }

    private void ThrowIfAlreadyMapped(Type targetType)
    {
        if (_keyMap.ContainsKey(targetType) || _exampleKeyMap.ContainsKey(targetType))
        {
            throw new InvalidOperationException($"Key for {targetType} is already mapped");
        }
    }

    private static void ThrowIfNotClass(string argName, Type targetType)
    {
        if (targetType.IsInterface || targetType.IsValueType)
        {
            throw new ArgumentException($"{argName} must be a class");
        }
    }

    private void ThrowIfConfigured()
    {
        if (ServiceDescriptors is null)
        {
            throw new InvalidOperationException($"{typeof(Container)} is already configured");
        }
    }

    private void MapExamples()
    {
        if (_exampleKeyMap.Count > 0)
        {
            List<Type> notMapped = new();
            Stack<Type> stack = new();
            while (_exampleKeyMap.Count > 0)
            {
                if (stack.Count == 0)
                {
                    stack.Push(_exampleKeyMap.Keys.First());
                }
                if (!_keyMap.ContainsKey(stack.Peek()) && !_exampleKeyMap.ContainsKey(stack.Peek()))
                {
                    while (stack.Count > 0)
                    {
                        _exampleKeyMap.Remove(stack.Peek());
                        notMapped.Add(stack.Pop());
                    }
                }
                else if (_keyMap.ContainsKey(stack.Peek()))
                {
                    Dictionary<string, KeyDefinition> example = _keyMap[stack.Peek()];
                    while (stack.Count > 0)
                    {
                        _exampleKeyMap.Remove(stack.Peek());
                        _keyMap[stack.Pop()] = example;
                    }
                }
                else
                {
                    if (stack.Contains(_exampleKeyMap[stack.Peek()]))
                    {
                        throw new Exception($"Example loop detected: {_exampleKeyMap[stack.Peek()]}");
                    }
                    stack.Push(_exampleKeyMap[stack.Peek()]);
                }
            }
            if (stack.Count > 0)
            {
                notMapped.AddRange(stack);
            }
            if (notMapped.Count > 0)
            {
                List<string> list = notMapped.Select(t => t.ToString()).OrderBy(s => s).ToList();
                for (int i = list.Count - 1; i > 0; --i)
                {
                    if (list[i - 1] == list[i])
                    {
                        list.RemoveAt(i);
                    }
                }
                throw new Exception($"Keys not mapped for: {string.Join(", ", list)}");
            }
        }
    }

    private void CheckKeyMappings()
    {
        ServiceDescriptor[] registered = ServiceDescriptors!.Where(item => _registered.ContainsKey(item.ServiceType)).ToArray();
        List<Type> missed = new();
        foreach (Type type in _keyMap.Keys)
        {
            if (
                registered.Where(item => item.ImplementationType == type
                || item.ImplementationInstance is { } && item.ImplementationInstance.GetType() == type
                || item.ImplementationFactory is { } && item.ImplementationFactory.Method.ReturnType == type).Count() == 0
            )
            {
                missed.Add(type);
            }
        }
        if (missed.Count > 0)
        {
            throw new Exception($"Keys are mapped for not registered types: {string.Join(", ", missed.Select(t => t.ToString()).OrderBy(s => s))}");
        }
    }


}