namespace Net.Leksi.Pocota.Server;

public abstract class PocotaEntity
{
    private bool _isSerialized = false;
    private bool _isAccessCalculated = false;
    private readonly Dictionary<string, object?> _keyValues = [];
    private readonly Dictionary<string, object> _probes = [];
    private object? _entity;
    internal bool InitializingProperties { get; private set; }
    public ulong PocotaId { get; internal set; }
    public bool IsSerialized
    {
        get => _isSerialized;
        set
        {
            if (!value)
            {
                throw new InvalidOperationException();
            }
            if (!_isSerialized && value)
            {
                _isSerialized = true;
            }
        }
    }
    public bool IsAccessCalculated
    {
        get => _isAccessCalculated;
        set
        {
            if (!value)
            {
                throw new InvalidOperationException();
            }
            if (!_isAccessCalculated && value)
            {
                _isAccessCalculated = true;
            }
        }
    }
    public object? Entity
    {
        get => _entity;
        set
        {
            if (_entity is null && value is { })
            {
                _entity = value;
                InitializingProperties = true;
                InitProperties();
                InitializingProperties = false;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
    protected void AddKeyValue(string name, object? value)
    {
        _keyValues.Add(name, value);
    }
    protected abstract void InitProperties();
}
