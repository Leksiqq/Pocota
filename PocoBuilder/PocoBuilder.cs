using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Core;
using System.Text;

namespace Net.Leksi.Pocota;

/// <summary>
/// <para xml:lang="ru">
/// Класс для построения объекта, ограниченного применённым интерфейсом
/// </para>
/// <para xml:lang="en">
/// Class for constructing an object limited by the applied interface
/// </para>
/// </summary>
public class PocoBuilder
{
    private const string Slash = "/";

    private readonly IServiceProvider _serviceProvider;
    private readonly TypesForest _typesForest;
    private readonly ObjectCache _objectCache;
    private readonly Dictionary<Type, object> _probeObjects = new();

    public PocoBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _typesForest = serviceProvider.GetRequiredService<TypesForest>();
        _objectCache = serviceProvider.GetRequiredService<ObjectCache>();
    }

    public T Build<T>(ValueNodeEventHandler onNode, T? target = null)
        where T : class
    {
        return (T)Build(typeof(T), onNode, target);
    }

    public object Build(Type type, ValueNodeEventHandler onNode, object? target = null)
    {
        TypeNode typeNode = _typesForest.GetTypeNode(type);
        if(target is null)
        {
            target = _serviceProvider.GetRequiredService(type);
        }
        bool walkIsInterrupted = _typesForest.WalkTree(target, type, args =>
        {
            if(args.Kind is ValueNodeKind.Node && args.Value is null)
            {
                if (_probeObjects.TryGetValue(args.NominalType, out object? probe))
                {
                    _probeObjects.Remove(args.NominalType);
                    args.Value = probe;
                }
                else
                {
                    args.Value = _serviceProvider.GetRequiredService(args.NominalType);
                }
            }
            onNode?.Invoke(args);
            if (args.Path == Slash)
            {
                target = args.Value;
            }
            if (args.IsLastKeyField)
            {
                if (_objectCache.TryGet(args.NominalType, target!, out object? cachedObject))
                {
                    _probeObjects.TryAdd(args.NominalType, target!);
                    target = cachedObject;
                    args.IsInterrupted = true;
                }
            }
        }, true);
        if (!walkIsInterrupted)
        {
            _objectCache.Add(type, target!);
        }
        return target!;
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Генерирует скелет для хэндлера, соответствующего применённому интерфейсу
    /// </para>
    /// <para xml:lang="en">
    /// Generates a skeleton for the handler corresponding to the applied interface
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// <para xml:lang="ru">
    /// Интерфейс, применяемый для генерации
    /// </para>
    /// <para xml:lang="en">
    /// Interface used to generate
    /// </para>
    /// </typeparam>
    /// <returns>
    /// <para xml:lang="ru">
    /// Исходный код, который можно скопировать и вставить в подходящее место
    /// </para>
    /// <para xml:lang="en">
    /// Source code that can be copied and pasted to a suitable location
    /// </para>
    /// </returns>
    public string GenerateHandlerSkeleton(Type type)
    {
        StringBuilder sb = new();
        sb.Append(@"
(ValueRequestEventArgs args) => {
    switch(args.Path) {");
        void eh(ValueNodeEventArgs args)
        {
            sb.Append(@$"
        case ""{args.Path}"":");
            if (args.Kind is ValueNodeKind.PrimaryKey)
            {
                sb.Append(@"
            args.Value = ...;");
                args.IsCommited = true;
            }
            else if (args.Kind is ValueNodeKind.Leaf)
            {
                sb.Append(@"
            //args.Value = ...;");
                args.IsCommited = true;
            }
            else
            {
                if (args.IsNullable)
                {
                    sb.Append(@"
            //args.Value = ...;
            //args.Value = null;");
                }
                else
                {
                    sb.Append(@"
            //args.Value = ...;
            //args.IsCommited = true;");
                }
            }
            sb.Append(@"
            break;");
        }
        _typesForest.WalkTree(_serviceProvider.GetRequiredService(type), type, eh);
        sb.AppendLine(@"
    }
}");
        return sb.ToString();
    }


}
