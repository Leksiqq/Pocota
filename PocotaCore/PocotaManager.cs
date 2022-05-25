using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Net.Leksi.Pocota.Core;

public class PocotaManager : IServiceProvider, IServiceCollection
{
    private readonly Dictionary<Type, int> _registered = new();
    private readonly Dictionary<Type, Dictionary<string, KeyDefinition>> _keyMap = new();
    private readonly Dictionary<Type, Type> _exampleKeyMap = new();
    private readonly ConditionalWeakTable<object, object[]> _attachedKeys = new();

    internal IServiceProvider? ServiceProvider { get; set; } = null;
    internal IServiceCollection? ServiceDescriptors { get; set; } = null;

    public bool IsRegistered(Type sourceType)
    {
        return _registered.ContainsKey(sourceType);
    }

    public IEnumerable<Type> GetRegistered()
    {
        return _registered.Keys;
    }

    public object? GetService(Type serviceType)
    {
        return ServiceProvider?.GetService(serviceType);
    }

    public KeyRing? GetKeyRing(object source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        KeyRing? keyRing = null;
        Type? current = source.GetType();
        while (current is { } && !_keyMap.ContainsKey(current))
        {
            current = current!.BaseType;
        }
        if (current is { } && _keyMap.ContainsKey(current))
        {
            keyRing = new KeyRing(this);
            keyRing.Source = source;
            if (_attachedKeys.TryGetValue(source, out object[]? key))
            {
                keyRing.Key = key;
            }
            else
            {
                keyRing.Key = new object[_keyMap[current].Count];
                _attachedKeys.Add(source, keyRing.Key);
                keyRing.IsNew = false;
            }
        }
        return keyRing;
    }

    public void Add(ServiceDescriptor item)
    {
        ThrowIfConfigured();
        ThrowIfNotTransient(item);
        _registered[item.ServiceType] = 1;
        ServiceDescriptors!.Add(item);
    }

    public void AddKeyMapping(Type targetType, IDictionary<string, Type> fieldDefinitions)
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

    public void AddKeyMapping(Type targetType, Type example)
    {
        ThrowIfConfigured();
        ThrowIfNotClass(nameof(targetType), targetType);
        ThrowIfNotClass(nameof(example), example);
        ThrowIfAlreadyMapped(targetType);
        _exampleKeyMap[targetType] = example;
    }

    internal int GetFieldIndex(Type type, string fieldName)
    {
        return _keyMap.ContainsKey(type) && _keyMap[type].ContainsKey(fieldName) ? _keyMap[type][fieldName].Index : -1;
    }

    internal static void AddPocotaCore(IServiceCollection services, Action<IServiceCollection> configure)
    {
        PocotaManager instance = new();
        services.AddSingleton<PocotaManager>(serviceProvider =>
        {
            instance.ServiceProvider = serviceProvider;
            return instance;
        });
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
            throw new InvalidOperationException($"{typeof(PocotaManager)} is already configured");
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
        ServiceDescriptor[] registered = ServiceDescriptors.Where(item => _registered.ContainsKey(item.ServiceType)).ToArray();
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

    #region Unused
    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public ServiceDescriptor this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int IndexOf(ServiceDescriptor item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, ServiceDescriptor item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(ServiceDescriptor item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(ServiceDescriptor item)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
    #endregion Unused

}