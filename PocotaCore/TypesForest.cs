using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text;

namespace Net.Leksi.Pocota.Core;

/// <summary>
/// <para xml:lang="ru">
/// "Лес" деревьев "свойств и типов" - служит для построения и хранения таких деревьев
/// </para>
/// <para xml:lang="en">
/// "Forest" of trees of "properties and types" - serves to build and store such trees
/// </para>
/// </summary>
public class TypesForest
{

    private const string Slash = "/";
    private const string Dot = ".";
    private const string _nullableAttributeName = "NullableAttribute";

    private static readonly PropertyNodeComparer _propertyNodeComparer = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly Container _container;

    private readonly Dictionary<Type, TypeNode> _typeTrees = new();

    /// <summary>
    /// <para xml:lang="ru">
    /// Инициализирует экземпляр класса <see cref="TypesForest"/> с внедрением провайдера служб.
    /// </para>
    /// <para xml:lang="en">
    /// Initializes an instance of the <see cref="TypesForest"/> class with a service provider injection.
    /// </para>
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public TypesForest(IServiceProvider serviceProvider) =>
        (_serviceProvider, _container) =
            (serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)), serviceProvider.GetRequiredService<Container>());
    /// <summary>
    /// <para xml:lang="ru">
    /// Возвращает корневой узел дерева, соответствующего указанному типу.
    /// Если такого дерева в лесу нет, сажает его, выращивает и возвращает
    /// </para>
    /// <para xml:lang="en">
    /// Returns the root node of the tree corresponding to the specified type.
    /// If there is no such tree in the forest, plant it, grow it and return it
    /// </para>
    /// </summary>
    /// <param name="type">
    /// <para xml:lang="ru">
    /// Запрашиваемый тип
    /// </para>
    /// <para xml:lang="en">
    /// Requested type
    /// </para>
    /// </param>
    /// <returns></returns>
    public TypeNode GetTypeNode(Type type)
    {
        lock (type)
        {
            if (!_typeTrees.ContainsKey(type))
            {
                _typeTrees[type] = new TypeNode { Type = type, ChildNodes = new List<PropertyNode>() };
                ConfigureTypeNode(_typeTrees[type]);
                //_typeTrees[type].ValueRequests = new List<ValueNode>();
                //CollectValueRequests1(_typeTrees[type], _typeTrees[type].ValueRequests!);
                CollectValueRequests(_typeTrees[type]);
            }
        }
        return _typeTrees[type];
    }

    /// <summary>
    /// <para xml:lang="ru">
    /// Ищет в лесу <see cref="PropertyNode"/>
    /// </para>
    /// <para xml:lang="en">
    /// Searches the forest for <see cref="PropertyNode"/>
    /// </para>
    /// </summary>
    /// <param name="typeNode">
    /// <para xml:lang="ru">
    /// <see cref="TypeNode"/>, содержащий искомое свойство
    /// </para>
    /// <para xml:lang="en">
    /// <see cref="TypeNode"/> containing the desired property
    /// </para>
    /// </param>
    /// <param name="propertyName">
    /// <para xml:lang="ru">
    /// Имя искомого свойства
    /// </para>
    /// <para xml:lang="en">
    /// Name of the searched property
    /// </para>
    /// </param>
    /// <returns></returns>
    public PropertyNode? FindPropertyNode(TypeNode typeNode, string propertyName)
    {
        return typeNode.ChildNodes?.Find(propertyNode => propertyNode.Name == propertyName);
    }

    /// <summary>
    /// <para xml:lang="ru">
    /// Обновляет дерево целевого объекта деревом исходного объекта по шаблону применяемого интерфейса
    /// </para>
    /// <para xml:lang="en">
    /// Updates the target object's tree with the source object's tree based on the template of the applied interface
    /// </para>
    /// </summary>
    /// <param name="sourceType">
    /// <para xml:lang="ru">
    /// Применяемый интерфейс
    /// </para>
    /// <para xml:lang="en">
    /// Applied interface
    /// </para>
    /// </param>
    /// <param name="source">
    /// <para xml:lang="ru">
    /// Исходный объект
    /// </para>
    /// <para xml:lang="en">
    /// Source object
    /// </para>
    /// </param>
    /// <param name="target">
    /// <para xml:lang="ru">
    /// Целевой объект
    /// </para>
    /// <para xml:lang="en">
    /// Target object
    /// </para>
    /// </param>
    public void Inject(Type sourceType, object source, object target)
    {
        TypeNode typeNode = GetTypeNode(sourceType);
        if (typeNode.ChildNodes is { })
        {
            foreach (PropertyNode propertyNode in typeNode.ChildNodes)
            {
                object? sourceValue = propertyNode.PropertyInfo?.GetValue(source);
                if (sourceValue is null)
                {
                    propertyNode.PropertyInfo?.SetValue(target, null);
                }
                else
                {
                    if (propertyNode.TypeNode.ChildNodes is { } children)
                    {
                        object? targetValue = propertyNode.PropertyInfo?.GetValue(target);
                        if (targetValue is null)
                        {
                            targetValue = _serviceProvider.GetRequiredService(propertyNode.TypeNode.Type);
                            propertyNode.PropertyInfo?.SetValue(target, targetValue);
                        }
                        Inject(propertyNode.TypeNode.Type, sourceValue, targetValue);
                    }
                    else
                    {
                        propertyNode.PropertyInfo?.SetValue(target, sourceValue);
                    }
                }

            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type"></param>
    /// <param name="onProperty"></param>
    /// <param name="afterPrimaryKey"></param>
    /// <param name="afterNode"></param>
    /// <param name="withUpdate"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void WalkTree(object source, Type type, ValueNodeEventHandler onProperty, ValueNodeEventHandler? afterPrimaryKey = null,
        ValueNodeEventHandler? afterNode = null, bool withUpdate = false)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        TypeNode typeNode = GetTypeNode(type);
        int waitForLevel = -1;
        KeyRing? keyRing = null;
        Dictionary<string, Type>? keyDefinition = null;
        Stack<ValueNodeEventArgs> targets = new();
        int keyPosition = -1;
        ValueNodeEventArgs args = null!;
        ValueNode? lastNodeRequest = null;
        object? lastNode = null;

        foreach (ValueNode request in typeNode.ValueRequests!)
        {
            if (waitForLevel == -1 || request.Level == waitForLevel)
            {
                waitForLevel = -1;
                while (request.Level < targets.Count)
                {
                    args = targets.Pop();
                    afterNode?.Invoke(args);
                }
                if (request.Kind is ValueNodeKind.PrimaryKey)
                {
                    if (keyDefinition is null)
                    {
                        keyRing = targets.Peek() is { } obj ? _container.GetKeyRing(obj.Value) : null;
                        keyDefinition = _container.GetPrimaryKeyDefinition(targets.Peek().ActualType);
                        keyPosition = 0;
                    }
                    ++keyPosition;
                    string keyFieldName = request.Path.Substring(request.Path.LastIndexOf(Slash) + 1);
                    args = new()
                    {
                        Path = request.Path,
                        Value = keyRing?[keyFieldName] ?? null,
                        NominalType = keyDefinition![keyFieldName],
                        ActualType = keyDefinition![keyFieldName],
                        Kind = request.Kind,
                        Level = request.Level,
                    };
                    onProperty?.Invoke(args);
                    if (withUpdate)
                    {
                        if (args.Value is null)
                        {
                            throw new InvalidOperationException($"At not nullable request \"{args.Path}\" null can not be assigned.");
                        }
                        keyRing![keyFieldName] = args.Value;
                    }
                    if (afterPrimaryKey is { } && targets.Peek() is { } target && keyPosition == keyDefinition.Count)
                    {
                        afterPrimaryKey.Invoke(target);
                        if (target.IsCommited)
                        {
                            waitForLevel = request.Level - 1;
                            try
                            {
                                lastNodeRequest!.PropertyNode!.PropertyInfo!.SetValue(lastNode, target.Value);
                            }
                            catch
                            {
                                Console.WriteLine(lastNodeRequest);
                                Console.WriteLine(lastNode);
                                Console.WriteLine(target.Value);
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    if (keyDefinition is { })
                    {
                        keyRing = null;
                        keyDefinition = null;
                        keyPosition = -1;
                    }
                    object? value = request.Level == 0 ? source : (targets.Peek().Value is { } obj ? request.PropertyNode!.PropertyInfo!.GetValue(obj) : null);
                    args = new()
                    {
                        Path = request.Path,
                        Value = value,
                        NominalType = request.PropertyNode!.TypeNode.Type,
                        ActualType = request.PropertyNode!.TypeNode.ActualType,
                        IsNullable = request.PropertyNode!.IsNullable,
                        Kind = request.Kind,
                        Level = request.Level,
                    };
                    onProperty?.Invoke(args);
                    if (withUpdate)
                    {
                        if (args.Value is null && !request.PropertyNode!.IsNullable)
                        {
                            throw new InvalidOperationException($"At not nullable request \"{args.Path}\" null can not be assigned.");
                        }
                        if (args.Value != value)
                        {
                            value = args.Value;
                            request.PropertyNode!.PropertyInfo!.SetValue(targets.Peek().Value, value);
                        }
                    }
                    if (request.Kind is ValueNodeKind.Node)
                    {
                        if (request.Level > 0)
                        {
                            lastNodeRequest = request;
                            lastNode = targets.Peek().Value;
                        }
                        if (args.IsCommited || args.Value is null)
                        {
                            waitForLevel = request.Level;
                            afterNode?.Invoke(args);
                        }
                        else
                        {
                            targets.Push(args);
                        }
                    }
                }
            }
        }
        while (targets.Count > 0)
        {
            args = targets.Pop();
            afterNode?.Invoke(args);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public string TreeToString<T>(object source) where T : class => TreeToString(source, typeof(T));
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string TreeToString(object source) => TreeToString(source, source?.GetType() ?? typeof(object));
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public string TreeToString(object source, Type type)
    {
        StringBuilder result = new();
        WalkTree(source, type, args =>
        {
            result.Append(args.Path).Append("=");
            if (args.Kind is ValueNodeKind.PrimaryKey)
            {
                result.Append(args.Value is null ? "<undefined>" : args.Value);
            }
            else if (args.Kind is ValueNodeKind.Node)
            {
                if (args.Value is { })
                {
                    result.Append("{").Append(args.NominalType.ToString()).Append("}");
                }
                else
                {
                    result.Append("null");
                    args.IsCommited = true;
                }
            }
            else
            {
                result.Append(args.Value is null ? "null" : args.Value);
            }
#if DEBUG
            result.AppendLine($" level: {args.Level}");
#else
            result.AppendLine();
#endif
        }
#if DEBUG
        , obj => result.AppendLine("after key")
        , obj => result.AppendLine("after inst")
#endif
        );
        return result.ToString();
    }

    private class Holder
    {
        internal TypeNode TypeNode = null!;
        internal int CurrentChildPosition = 0;
        internal string Path = string.Empty;
    }

    private void ConfigureTypeNode(TypeNode typeNode)
    {
        List<PropertyInfo> properties = new();
        CollectProperties(typeNode, properties);

        typeNode.ActualType = _container.ContainsServiceType(typeNode.Type) ?
            _container.GetActualType(typeNode.Type)! : typeNode.Type;
        List<PropertyInfo> actualProperties = new();
        CollectActualProperties(typeNode, actualProperties);

        CollectChildNodes(typeNode, properties, actualProperties);
    }

    private void CollectValueRequests(TypeNode typeNode)
    {
        Stack<Holder> stack = new();
        Stack<PropertyInfo?> loopControl = new();
        loopControl.Push(null);
        stack.Push(new Holder { TypeNode = typeNode, CurrentChildPosition = 0 });
        typeNode.ValueRequests = new List<ValueNode>();
        typeNode.ValueRequests.Add(new ValueNode
        {
            Path = Slash,
            PropertyNode = new PropertyNode { TypeNode = typeNode },
            Level = 0
        });
        while (stack.Count > 0)
        {
            Holder holder = stack.Peek();
            int level = stack.Count - 1;
            if(holder.CurrentChildPosition == 0)
            {
                if (_container.GetPrimaryKeyDefinition(holder.TypeNode.ActualType) is Dictionary<string, Type> keyDefinitions)
                {
                    foreach (KeyValuePair<string, Type> entry in keyDefinitions)
                    {
                        typeNode.ValueRequests.Add(new ValueNode
                        {
                            Path = holder.Path + Slash + entry.Key,
                            KeyFieldType = entry.Value,
                            Level = level + 1
                        });
                    }
                }
            }
            for(; holder.CurrentChildPosition < holder.TypeNode.ChildNodes!.Count && holder.TypeNode.ChildNodes![holder.CurrentChildPosition].IsLeaf; ++holder.CurrentChildPosition)
            {
                typeNode.ValueRequests.Add(new ValueNode
                {
                    Path = holder.Path + Slash + holder.TypeNode.ChildNodes![holder.CurrentChildPosition].Name,
                    PropertyNode = holder.TypeNode.ChildNodes![holder.CurrentChildPosition],
                    Level = level + 1
                });
            }
            if(holder.CurrentChildPosition < holder.TypeNode.ChildNodes!.Count)
            {
                if (!loopControl.Contains(holder.TypeNode.ChildNodes![holder.CurrentChildPosition].PropertyInfo))
                {
                    loopControl.Push(holder.TypeNode.ChildNodes![holder.CurrentChildPosition].PropertyInfo);
                    string path = holder.Path + Slash + holder.TypeNode.ChildNodes![holder.CurrentChildPosition].Name;
                    typeNode.ValueRequests.Add(new ValueNode
                    {
                        Path = path,
                        PropertyNode = holder.TypeNode.ChildNodes![holder.CurrentChildPosition],
                        Level = level + 1
                    });
                    stack.Push(new Holder
                    {
                        TypeNode = holder.TypeNode.ChildNodes![holder.CurrentChildPosition].TypeNode,
                        CurrentChildPosition = 0,
                        Path = path
                    });
                }
                ++holder.CurrentChildPosition;
            }
            else
            {
                stack.Pop();
                loopControl.Pop();
            }
        }
    }

    private void CollectValueRequests1(TypeNode typeNode, List<ValueNode> requests)
    {
        int level = 0;
        StringBuilder path = new();
        requests.Add(new ValueNode
        {
            Path = Slash,
            PropertyNode = new PropertyNode { TypeNode = typeNode },
            Level = level
        });
        if (_container.GetPrimaryKeyDefinition(typeNode.ActualType) is Dictionary<string, Type> keyDefinitions)
        {
            foreach (KeyValuePair<string, Type> entry in keyDefinitions)
            {
                ++level;
                int pathLength = path.Length;
                path.Append(Slash).Append(entry.Key);
                ValueNode request = new ValueNode
                {
                    Path = path.ToString(),
                    KeyFieldType = entry.Value,
                    Level = level
                };
                requests.Add(request);
                path.Length = pathLength;
                --level;
            }
        }
        ++level;
        Stack<PropertyInfo?> loopControl = new();
        foreach (PropertyNode propertyNode in typeNode.ChildNodes!)
        {
            loopControl.Push(propertyNode.PropertyInfo);
            int pathLength = path.Length;
            path.Append(Slash).Append(propertyNode.PropertyInfo!.Name);
            ValueNode request = new ValueNode
            {
                Path = path.ToString(),
                PropertyNode = propertyNode,
                Level = level
            };
            requests.Add(request);
            if (!propertyNode.IsLeaf)
            {
                List<ValueNode> range = new();
                foreach (ValueNode req in propertyNode.TypeNode.ValueRequests!)
                {
                    if (req.Path != Slash && (req.PropertyNode is null || !loopControl.Contains(req.PropertyNode.PropertyInfo)))
                    {
                        loopControl.Push(req.PropertyNode?.PropertyInfo);
                        int pathLength1 = path.Length;
                        path.Append(req.Path);
                        request = new ValueNode
                        {
                            Path = path.ToString(),
                            Level = level + req.Level
                        };
                        if (req.Kind is ValueNodeKind.PrimaryKey)
                        {
                            request.KeyFieldType = req.KeyFieldType;
                        }
                        else
                        {
                            request.PropertyNode = req.PropertyNode;
                        }
                        range.Add(request);
                        path.Length = pathLength1;
                        loopControl.Pop();
                    }
                }
                requests.AddRange(range);
            }
            path.Length = pathLength;
            loopControl.Pop();
        }
        --level;
    }

    private void CollectChildNodes(TypeNode typeNode, List<PropertyInfo> properties,
        List<PropertyInfo> actualProperties)
    {
        foreach (PropertyInfo propertyInfo in actualProperties)
        {
            PropertyInfo? actualProperty = null;
            if (propertyInfo.Name.Contains(Dot))
            {
                if (propertyInfo.Name.StartsWith(typeNode.Type.FullName!)
                    && propertyInfo.Name.LastIndexOf(Dot) == typeNode.Type.FullName!.Length
                )
                {
                    actualProperty = actualProperties.Where(
                        v => v.Name == propertyInfo.Name.Substring(propertyInfo.Name.LastIndexOf(Dot) + 1)
                    ).FirstOrDefault();
                }
            }
            else
            {
                PropertyInfo? sourceProperty = properties.Where(
                    v => v.Name == propertyInfo.Name && v.PropertyType == propertyInfo.PropertyType
                ).FirstOrDefault();
                if (sourceProperty is PropertyInfo)
                {
                    actualProperty = propertyInfo;
                }
            }
            if (actualProperty is PropertyInfo && actualProperty.CanWrite)
            {
                bool foundTypeNode = _typeTrees.ContainsKey(propertyInfo.PropertyType);
                var newPropertyNode = new PropertyNode
                {
                    Name = propertyInfo.Name.Contains(Dot)
                        ? propertyInfo.Name.Substring(propertyInfo.Name.LastIndexOf(Dot) + 1)
                        : propertyInfo.Name,
                    PropertyInfo = actualProperty,
                    TypeNode = _container.ContainsServiceType(propertyInfo.PropertyType)
                        ? GetTypeNode(propertyInfo.PropertyType)
                        : new TypeNode { Type = propertyInfo.PropertyType, ActualType = propertyInfo.PropertyType },
                    IsNullable = (propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(propertyInfo.PropertyType) is Type)
                        || propertyInfo.GetCustomAttributes().Any(a => a.GetType().Name.Contains(_nullableAttributeName))
                };
                typeNode.ChildNodes!.Add(newPropertyNode);
            }
        }
        typeNode.ChildNodes!.Sort(_propertyNodeComparer);
    }

    private void CollectActualProperties(TypeNode typeNode, List<PropertyInfo> actualProperties)
    {
        Type currentType = typeNode.ActualType;
        List<Type> considered = new();
        while (currentType != typeof(object))
        {
            foreach (PropertyInfo propertyInfo in currentType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                actualProperties.Add(propertyInfo);
            }
            currentType = currentType.BaseType!;
        }
    }

    private void CollectProperties(TypeNode typeNode, List<PropertyInfo> properties)
    {
        Queue<Type> queue = new();
        List<Type> considered = new();
        queue.Enqueue(typeNode.Type);
        while (queue.Count > 0)
        {
            Type subType = queue.Dequeue();
            foreach (Type subInterface in subType.GetInterfaces())
            {
                if (!considered.Contains(subInterface))
                {
                    queue.Enqueue(subInterface);
                }
                considered.Add(subInterface);
            }
            foreach (PropertyInfo propertyInfo in subType.GetProperties())
            {
                properties.Add(propertyInfo);
            }
        }
    }

}
