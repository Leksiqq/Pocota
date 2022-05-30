using Net.Leksi.Pocota.Core;

namespace Net.Leksi.Pocota;

/// <summary>
/// <para xml:lang="ru">
/// Потомок <see cref="EventArgs"/>, для передачи в <see cref="ValueNodeEventHandler"/>
/// </para>
/// <para xml:lang="en">
/// Child of <see cref="EventArgs"/>, to be passed to <see cref="ValueNodeEventHandler"/>
/// </para>
/// </summary>
public class ValueNodeEventArgs : EventArgs
{
    /// <summary>
    /// <para xml:lang="ru">
    /// Значение текущего узла или листа дерева объекта
    /// </para>
    /// <para xml:lang="en">
    /// The value of the current node or leaf of the object tree
    /// </para>
    /// </summary>
    public object? Value { get; set; }
    /// <summary>
    /// <para xml:lang="ru">
    /// Указывает, принимает ли соответствующее свойство значения <code>null</code>
    /// </para>
    /// <para xml:lang="en">
    /// Indicates whether the corresponding property accepts <code>null</code> values
    /// </para>
    /// </summary>
    public bool IsNullable { get; internal set; } = false;
    /// <summary>
    /// <para xml:lang="ru">
    /// Указывает, является ли данный узел листом
    /// </para>
    /// <para xml:lang="en">
    /// Indicates if this node is a leaf
    /// </para>
    /// </summary>
    public ValueNodeKind Kind { get; internal set; } = ValueNodeKind.Node;
    /// <summary>
    /// <para xml:lang="ru">
    /// Абсолютный путь от корня дерева объекта
    /// </para>
    /// <para xml:lang="en">
    /// Absolute path from the root of the object tree
    /// </para>
    /// </summary>
    public string Path { get; internal set; } = null!;
    /// <summary>
    /// <para xml:lang="ru">
    /// Тип узла дерева применённого интерфейса, соответствующего текущему узлу дерева объекта
    /// </para>
    /// <para xml:lang="en">
    /// The type of the tree node of the applied interface corresponding to the current tree node of the object
    /// </para>
    /// </summary>
    public Type NominalType { get; internal set; } = null!;
    /// <summary>
    /// <para xml:lang="ru">
    /// Тип текущего узла дерева объекта
    /// </para>
    /// <para xml:lang="en">
    /// Type of the current node of the object tree
    /// </para>
    /// </summary>
    public Type ActualType { get; internal set; } = null!;
    /// <summary>
    /// <para xml:lang="ru">
    /// Сигнализирует, что текущий узел дерева объекта был завершён
    /// </para>
    /// <para xml:lang="en">
    /// Signals that the current object tree node has been commited
    /// </para>
    /// </summary>
    public bool IsCommited { get; set; } = false;
    /// <summary>
    /// <para xml:lang="ru">
    /// Сигнализирует, что текущий лист дерева объекта является последним полем первичного ключа
    /// </para>
    /// <para xml:lang="en">
    /// Signals that the current leaf of the object tree is the last field of the primary key
    /// </para>
    /// </summary>
    public bool IsLastKeyField { get; internal set; } = false;
    /// <summary>
    /// <para xml:lang="ru">
    /// Сигнализирует, что текущий обход дерева объекта был прерван
    /// </para>
    /// <para xml:lang="en">
    /// Signals that the current object tree traversal has been interrupted
    /// </para>
    /// </summary>
    public bool IsInterrupted { get; set; } = false;
}
