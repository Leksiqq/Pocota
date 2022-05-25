using System.Reflection;

namespace Net.Leksi.Pocota.Core;

/// <summary>
/// <para xml:lang="ru">
/// Класс для сортировки <see cref="PropertyNode"/>
/// Первыми ставятся листовые свойства, затем узловые
/// Внутри каждого из указанных множеств сортируются по именам в алфавитном порядке
/// </para>
/// <para xml:lang="en">
/// Class for sorting <see cref="PropertyNode"/>
/// Leaf properties are put first, then node properties
/// Within each of the specified sets are sorted by name in alphabetical order
/// </para>
/// </summary>
public class PropertyNodeComparer : IComparer<PropertyNode>
{
    /// <inheritdoc/>
    public int Compare(PropertyNode? x, PropertyNode? y)
    {
        if (x == y)
        {
            return 0;
        }
        if (x is null)
        {
            return -1;
        }
        if (y is null)
        {
            return 1;
        }
        if (x.TypeNode.ChildNodes is null && y.TypeNode.ChildNodes is { })
        {
            return -1;
        }
        if (x.TypeNode.ChildNodes is { } && y.TypeNode.ChildNodes is null)
        {
            return 1;
        }
        return string.Compare(x.Name, y.Name);
    }
}
