namespace Net.Leksi.Pocota.Server;

public abstract class PocotaEntity
{
    private bool _isSerialized = false;
    private object? _entity;
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
    public object? Entity
    {
        get => _entity;
        set
        {
            if (_entity is null && value is { })
            {
                _entity = value;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
    protected abstract void InitProperties();
}
