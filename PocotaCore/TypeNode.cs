using System.Text;

namespace Net.Leksi.Pocota.Core;

/// <summary>
/// <para xml:lang="ru">
/// Узел дерева "свойств и типов" для кеширования рефлексии и программирования построения объектов
/// </para>
/// <para xml:lang="en">
/// "properties and types" tree node for reflection caching and object building programming
/// </para>
/// </summary>
public class TypeNode
{
    /// <summary>
    /// <para xml:lang="ru">
    /// Тип свойства, определённый в интерфейсе
    /// </para>
    /// <para xml:lang="en">
    /// Property type defined in the interface
    /// </para>
    /// </summary>
    public Type Type { get; internal set; } = null!;
    /// <summary>
    /// <para xml:lang="ru">
    /// Тип свойства, определённый в классе
    /// </para>
    /// <para xml:lang="en">
    /// Property type defined in the class
    /// </para>
    /// </summary>
    public Type ActualType { get; internal set; } = null!;
    /// <summary>
    /// <para xml:lang="ru">
    /// Список дочерних узлов, соответствующих свойствам, определённым в интерфейсе
    /// </para>
    /// <para xml:lang="en">
    /// List of child nodes corresponding to the properties defined in the interface
    /// </para>
    /// </summary>
    public List<PropertyNode>? ChildNodes { get; internal set; }
    /// <summary>
    /// <para xml:lang="ru">
    /// Количество ключевых свойств, определённых в классе
    /// </para>
    /// <para xml:lang="en">
    /// Number of key properties defined in the class
    /// </para>
    /// </summary>
    public List<ValueRequest>? ValueRequests { get; internal set; } = null;

}
