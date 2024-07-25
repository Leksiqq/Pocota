using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Server;

public abstract class PocotaEntity
{
    private bool _isSerialized = false;
    private bool _isAccessCalculated = false;
    private AccessKind _entityAccess = AccessKind.Full;
    private object? _entity;
    internal bool InitializingProperties { get; private set; }
    public ulong PocotaId { get; internal set; }
    public virtual AccessKind Access
    {
        get => _entityAccess;
        set
        {
            if (
                (
                    value == AccessKind.Forbidden
                    || value == AccessKind.Readonly
                    || value == AccessKind.Full
                )
                && (
                    value <= _entityAccess 
                ) 
            )
            {
                _entityAccess = value;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
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
            if (_entity == null && value != null)
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
    protected abstract void InitProperties();
}
