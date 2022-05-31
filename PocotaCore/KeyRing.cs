using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Net.Leksi.Pocota.Core;
/// <summary>
/// <para xml:lang="ru">
/// Аксессор к первичному ключу объекта.
/// </para>
/// <para xml:lang="en">
/// Accessor to the object's primary key.
/// </para>
/// </summary>
public class KeyRing: IReadOnlyDictionary<string, object>
{
    private readonly Container _manager;
    private readonly Dictionary<string, KeyDefinition> _keyDefinition;

    internal object[] PrimaryKey { get; set; } = null!;

    /// <summary>
    /// <para xml:lang="ru">
    /// <c>true</c> означает, что всем полям первичного ключа присвоены значения.
    /// </para>
    /// <para xml:lang="en">
    /// <c>true</c> means that all primary key fields have values assigned.
    /// </para>
    /// </summary>
    public bool IsAssigned
    {
        get
        {
            return PrimaryKey is { } && PrimaryKey.All(v => v is { });
        }
    }
    /// <summary>
    /// <para xml:lang="ru">
    /// Объект, к первичному ключу которого даётся доступ.
    /// </para>
    /// <para xml:lang="en">
    /// The object whose primary key is being accessed.
    /// </para>
    /// </summary>
    public object? Source { get; internal set; } = null;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> Keys => _keyDefinition.Keys;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<object> Values => _keyDefinition.Values.Select(v => PrimaryKey[v.Index]);
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => _keyDefinition.Count;
    /// <summary>
    /// <para xml:lang="ru">
    /// Инициализирует экземпляр <see cref="KeyRing"/> менеджером первичных ключей и определением первичного ключа.
    /// </para>
    /// <para xml:lang="en">
    /// Initializes the <see cref="KeyRing"/> instance with a primary key manager and a primary key definition.
    /// </para>
    /// </summary>
    /// <param name="manager">
    /// <para xml:lang="ru">
    /// Менеджер первичных ключей.
    /// </para>
    /// <para xml:lang="en">
    /// Primary key manager.
    /// </para>
    /// </param>
    /// <param name="keyDefinition">
    /// <para xml:lang="ru">
    /// Определение первичного ключа.
    /// </para>
    /// <para xml:lang="en">
    /// Define the primary key.
    /// </para>
    /// </param>
    /// <summary>
    /// <inheritdoc/>
    /// <para xml:lang="ru">
    /// Доступ к полю первичного ключа. Запись можно произвести только один раз.
    /// </para>
    /// <para xml:lang="en">
    /// Access to the primary key field. Registration can only be done once.
    /// </para>
    /// </summary>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public object this[string fieldName]
    {
        get
        {
            return PrimaryKey[_keyDefinition[fieldName].Index];
        }
        set
        {
            if (PrimaryKey[_keyDefinition[fieldName].Index] is null)
            {
                PrimaryKey[_keyDefinition[fieldName].Index] = value;
                if (IsAssigned)
                {
                    _manager.Attach(this);
                }
            }
        }
    }
    internal KeyRing(Container manager, Dictionary<string, KeyDefinition> keyDefinition) => (_manager, _keyDefinition) = (manager, keyDefinition);
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool ContainsKey(string key) => _keyDefinition.ContainsKey(key);
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
    {
        value = default;
        KeyDefinition? keyDefinition;
        if(_keyDefinition.TryGetValue(key, out keyDefinition))
        {
            value = PrimaryKey[keyDefinition.Index];
            return true;
        }
        return false;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _keyDefinition.Select(v => new KeyValuePair<string, object>(v.Key, PrimaryKey[v.Value.Index])).GetEnumerator();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public KeyRing SetField(string name, object value)
    {
        this[name] = value;
        return this;
    }
}
