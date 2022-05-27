namespace Net.Leksi.Pocota.Core;

/// <summary>
/// <para xml:lang="ru">
/// Определяет разновидность запроса значения.
/// </para>
/// <para xml:lang="en">
/// Defines the kind of value request.
/// </para>
/// </summary>
public enum ValueRequestKind
{
    /// <summary>
    /// <para xml:lang="ru">
    /// Определяет запрос значения как первичный ключ или часть первичного ключа объекта.
    /// </para>
    /// <para xml:lang="en">
    /// Defines a value request as the primary key or part of the object's primary key.
    /// </para>
    /// </summary>
    PrimaryKey,
    /// <summary>
    /// <para xml:lang="ru">
    /// Определяет запрос значения как лист графа запросов.
    /// </para>
    /// <para xml:lang="en">
    /// Defines a value request as a leaf of a requests graph.
    /// </para>
    /// </summary>
    Leaf,
    /// <summary>
    /// <para xml:lang="ru">
    /// Определяет запрос значения как узел графа запросов.
    /// </para>
    /// <para xml:lang="en">
    /// Defines a value request as a node of a requests graph.
    /// </para>
    /// </summary>
    Node
}
