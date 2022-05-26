using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Net.Leksi.Pocota.Core;

public class KeyRing: IReadOnlyDictionary<string, object>
{
    private readonly Manager _manager;
    private readonly Dictionary<string, KeyDefinition> _entry;

    internal object[] PrimaryKey { get; set; } = null!;

    public object? Source { get; internal set; } = null;

    public bool IsReadonly
    {
        get
        {
            return PrimaryKey is { } && PrimaryKey.All(v => v is { });
        }
    }

    public IEnumerable<string> Keys => _entry.Keys;

    public IEnumerable<object> Values => _entry.Values.Select(v => PrimaryKey[v.Index]);

    public int Count => _entry.Count;

    internal KeyRing(Manager manager, Dictionary<string, KeyDefinition> entry)
    {
        _manager = manager;
        _entry = entry;
    }

    public object this[string fieldName]
    {
        get
        {
            return PrimaryKey[_entry[fieldName].Index];
        }
        set
        {
            if(PrimaryKey[_entry[fieldName].Index] is null)
            {
                PrimaryKey[_entry[fieldName].Index] = value;
                if (IsReadonly)
                {
                    _manager.Attach(this);
                }
            }
        }
    }

    public bool ContainsKey(string key) => _entry.ContainsKey(key);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
    {
        value = default;
        KeyDefinition? keyDefinition;
        if(_entry.TryGetValue(key, out keyDefinition))
        {
            value = PrimaryKey[keyDefinition.Index];
            return true;
        }
        return false;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _entry.Select(v => new KeyValuePair<string, object>(v.Key, PrimaryKey[v.Value.Index])).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
