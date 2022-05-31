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

    public PocoBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _typesForest = serviceProvider.GetRequiredService<TypesForest>();
    }

    public T Build<T>(ValueNodeEventHandler onNode, T? target = null)
        where T : class
    {
        return (T)Build(typeof(T), onNode, target);
    }

    public object Build(Type type, ValueNodeEventHandler onNode, object? target = null)
    {
        ObjectCache objectCache = _serviceProvider.GetRequiredService<ObjectCache>();
        Dictionary<Type, object> probeObjects = new(); 
        TypeNode typeNode = _typesForest.GetTypeNode(type);
        if(target is null)
        {
            target = _serviceProvider.GetRequiredService(type);
        }
        _typesForest.WalkTree(
            target, 
            type, 
            onProperty: args =>
            {
                if(args.Kind is ValueNodeKind.Node && args.Value is null)
                {
                    if (probeObjects.TryGetValue(args.NominalType, out object? probe))
                    {
                        probeObjects.Remove(args.NominalType);
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
            },
            afterPrimaryKey: args =>
            {
                if (objectCache.TryGet(args.NominalType, args.Value!, out object? cachedObject))
                {
                    probeObjects.TryAdd(args.NominalType, target!);
                    args.Value = cachedObject;
                    args.IsCommited = true;
                }
            },
            afterNode: args =>
            {
                if(args.Value is { })
                {
                    objectCache.Add(args.NominalType, args.Value);
                }
            },
            withUpdate: true
        );
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
    /// <param name="type">
    /// <para xml:lang="ru">
    /// Интерфейс, применяемый для генерации
    /// </para>
    /// <para xml:lang="en">
    /// Interface used to generate
    /// </para>
    /// </param>
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
                sb.Append(@"
            //args.Value = ...;");
                if (args.IsNullable)
                {
                    sb.Append(@"
            //args.Value = null;");
                }
                sb.Append(@"
            //args.IsCommited = true;");
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
    public string GenerateHandlerSkeleton<T>() where T : class
    {
        return GenerateHandlerSkeleton(typeof(T));
    }

}
