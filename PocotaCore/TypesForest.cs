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
    private readonly Container _manager;

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
        (_serviceProvider, _manager) =
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
        return GetTypeNode(type, null);
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

    public bool WalkTree(object source, Type type, ValueNodeEventHandler onNode, bool withUpdate = false)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        TypeNode typeNode = GetTypeNode(type);
        int waitForLevel = -1;
        KeyRing? keyRing = null;
        Dictionary<string, Type>? keyDefinition = null;
        int keyPos = -1;
        Stack<object?> targets = new();
        Stack<Type> types = new();
        targets.Push(source);
        types.Push(type);
        foreach (ValueNode request in typeNode.ValueRequests!)
        {
            if (waitForLevel == -1 || request.Level == waitForLevel)
            {
                waitForLevel = -1;
                ValueNodeEventArgs args = null!;
                if (request.Level == 0)
                {
                    args = new()
                    {
                        Path = request.Path,
                        Value = targets.Peek(),
                        NominalType = type,
                        ActualType = source.GetType(),
                        Kind = request.Kind,
                    };
                    onNode?.Invoke(args);
                }
                else
                {
                    while (request.Level < targets.Count)
                    {
                        targets.Pop();
                        types.Pop();
                    }
                    if (request.Kind is ValueNodeKind.PrimaryKey)
                    {
                        if (keyPos == -1)
                        {
                            keyRing = targets.Peek() is { } obj ? _manager.GetKeyRing(obj) : null;
                            keyDefinition = _manager.GetPrimaryKeyDefinition(types.Peek());
                            keyPos = 0;
                        }
                        keyPos++;
                        string keyFieldName = request.Path.Substring(request.Path.LastIndexOf(Slash) + 1);
                        args = new()
                        {
                            Path = request.Path,
                            Value = keyRing?[keyFieldName] ?? null,
                            NominalType = keyDefinition![keyFieldName],
                            ActualType = keyDefinition![keyFieldName],
                            Kind = request.Kind,
                            IsLastKeyField = keyPos == keyDefinition.Count,
                        };
                        onNode?.Invoke(args);
                        if (withUpdate)
                        {
                            if(args.Value is null)
                            {
                                throw new InvalidOperationException($"At not nullable request \"{ args.Path }\" null can not be assigned.");
                            }
                            keyRing![keyFieldName] = args.Value;
                        }
                    }
                    else
                    {
                        if (keyPos >= 0)
                        {
                            keyRing = null;
                            keyDefinition = null;
                            keyPos = -1;
                        }
                        object? value = targets.Peek() is { } obj ? request.PropertyNode!.PropertyInfo!.GetValue(obj) : null;
                        args = new()
                        {
                            Path = request.Path,
                            Value = value,
                            NominalType = request.PropertyNode!.TypeNode.Type,
                            ActualType = request.PropertyNode!.TypeNode.ActualType,
                            IsNullable = request.PropertyNode!.IsNullable,
                            Kind = request.Kind,
                        };
                        onNode?.Invoke(args);
                        if(withUpdate)
                        {
                            if (args.Value is null && !request.PropertyNode!.IsNullable)
                            {
                                throw new InvalidOperationException($"At not nullable request \"{args.Path}\" null can not be assigned.");
                            }
                            if(args.Value != value)
                            {
                                value = args.Value;
                                request.PropertyNode!.PropertyInfo!.SetValue(targets.Peek(), value);
                            }
                        }
                        if (request.Kind is ValueNodeKind.Node)
                        {
                            if (args.IsCommited)
                            {
                                waitForLevel = request.Level;
                            }
                            else
                            {
                                targets.Push(value);
                                types.Push(request.PropertyNode.TypeNode.Type);
                            }
                        }
                    }
                }
                if (args.IsInterrupted)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public string TreeToString<T>(object source) where T : class => TreeToString(source, typeof(T));

    public string TreeToString(object source) => TreeToString(source, source?.GetType() ?? typeof(object));

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
            else if(args.Kind is ValueNodeKind.Node)
            {
                if(args.Value is { })
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
            result.AppendLine();
        });
        return result.ToString();
    }

    private TypeNode GetTypeNode(Type type, Stack<Type>? stack)
    {
        if (!_typeTrees.ContainsKey(type))
        {
            if (stack is null)
            {
                stack = new Stack<Type>();
            }
            PlantTypeTree(type, stack);
        }
        return _typeTrees[type];
    }

    private bool PlantTypeTree(Type type, Stack<Type> stack)
    {
        stack.Push(type);
        if (!_manager.ContainsServiceType(type))
        {
            throw new ArgumentException($"{type} is not registered.");
        }
        bool result = !_typeTrees.ContainsKey(type);
        if (result)
        {
            lock (type)
            {
                result = !_typeTrees.ContainsKey(type);
                if (result)
                {
                    _typeTrees[type] = new TypeNode { Type = type, ChildNodes = new List<PropertyNode>() };
                    ConfigureTypeNode(_typeTrees[type], stack);

                    _typeTrees[type].ValueRequests = new List<ValueNode>();
                    CollectValueRequests(_typeTrees[type], _typeTrees[type].ValueRequests!, stack);
                }
            }
        }
        stack.Pop();
        return result;
    }

    private void ConfigureTypeNode(TypeNode typeNode, Stack<Type> stack)
    {
        List<PropertyInfo> properties = new();
        CollectProperties(typeNode, properties);

        typeNode.ActualType = _manager.ContainsServiceType(typeNode.Type) ?
            _manager.GetActualType(typeNode.Type)! : typeNode.Type;
        List<PropertyInfo> actualProperties = new();
        CollectActualProperties(typeNode, actualProperties);

        CollectChildNodes(typeNode, properties, actualProperties, stack);
    }

    private void CollectValueRequests(TypeNode typeNode, List<ValueNode> requests, Stack<Type> stack)
    {
        int level = 0;
        StringBuilder path = new();
        requests.Add(new ValueNode
        {
            Path = Slash,
            PropertyNode = new PropertyNode { TypeNode = typeNode },
            Level = level
        });
        if (_manager.GetPrimaryKeyDefinition(typeNode.ActualType) is Dictionary<string, Type> keyDefinitions)
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
        foreach (PropertyNode propertyNode in typeNode.ChildNodes!)
        {
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
                if (propertyNode.TypeNode.ValueRequests is null)
                {
                    var stackReverse = stack.ToList();
                    stackReverse.Reverse();
                    string stackView = string.Join(Slash, stackReverse);
                    throw new Exception($"Loop detected: {stackView}");
                }
                List<ValueNode> range = new();
                foreach (ValueNode req in propertyNode.TypeNode.ValueRequests!)
                {
                    if (req.Path != Slash)
                    {
                        if (req.Kind is not ValueNodeKind.PrimaryKey && ReferenceEquals(requests, propertyNode.TypeNode.ValueRequests))
                        {
                            break;
                        }
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

                    }
                }
                requests.AddRange(range);
            }
            path.Length = pathLength;
        }
        --level;
    }

    private void CollectChildNodes(TypeNode typeNode, List<PropertyInfo> properties,
        List<PropertyInfo> actualProperties, Stack<Type> stack)
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
                    TypeNode = _manager.ContainsServiceType(propertyInfo.PropertyType)
                        ? GetTypeNode(propertyInfo.PropertyType, stack)
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
