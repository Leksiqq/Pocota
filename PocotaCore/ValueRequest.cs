namespace Net.Leksi.Pocota.Core;
/// <summary>
/// <para xml:lang="ru">
/// Запрос на получение значения одного свойства при загрузке или сериализации объекта
/// </para>
/// <para xml:lang="en">
/// Request to get the value of one property when loading or serializing an object
/// </para>
/// </summary>
public class ValueRequest
{
    /// <summary>
    /// <para xml:lang="ru">
    /// Глубина данного узла по отношению к корню
    /// </para>
    /// <para xml:lang="en">
    /// Depth of this node relative to the root
    /// </para>
    /// </summary>
    public int Level { get; internal set; } = 0;

    /// <summary>
    /// <para xml:lang="ru">
    /// Информация о соответствующем запросу свойстве объекта. 
    /// Не используется при запросе PrimaryKey.
    /// </para>
    /// <para xml:lang="en">
    /// Information about the property of the object corresponding to the request.
    /// Not used for PrimaryKey request.
    /// </para>
    /// </summary>
    public PropertyNode? PropertyNode { get; internal set; } = null;

    /// <summary>
    /// <para xml:lang="ru">
    /// Тип поля первичного ключа объекта, соответствующего запросу.
    /// Используется только при запросе PrimaryKey.
    /// </para>
    /// <para xml:lang="en">
    /// The type of the primary key field of the object corresponding to the request.
    /// Used only for PrimaryKey request.
    /// </para>    /// </summary>
    public Type? KeyFieldType { get; internal set; } = null;
    /// <summary>
    /// <para xml:lang="ru">
    /// Абсолютный путь от корневого узла дерева объекта в стандартной нотации 
    /// </para>
    /// <para xml:lang="en">
    /// Absolute path from the root node of object&apos;s tree in standard notation
    /// </para>
    /// </summary>
    public string Path { get; internal set; } = string.Empty;

    /// <summary>
    /// <para xml:lang="ru">
    /// Разновидность запроса 
    /// </para>
    /// <para xml:lang="en">
    /// Request kind
    /// </para>
    /// </summary>
    public ValueRequestKind Kind => KeyFieldType is { } ? ValueRequestKind.PrimaryKey : (PropertyNode!.IsLeaf ? ValueRequestKind.Leaf: ValueRequestKind.Node);

#if DEBUG
    public override string ToString()
    {
        return $"{Path}, {(Kind switch { ValueRequestKind.PrimaryKey => "pk", ValueRequestKind.Leaf => "leaf", _ => "+" + PropertyNode!.TypeNode.Type })}, {Level}";
    }
#endif
}
