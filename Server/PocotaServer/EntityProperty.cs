using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Server;

public class EntityProperty
{
    private bool _isSent = false;
    protected AccessKind _propertyAccess = AccessKind.Full;
    private readonly PocotaEntity _entity;
    public virtual AccessKind Access 
    { 
        get => _entity.Access < _propertyAccess ? _entity.Access : _propertyAccess; 
        set
        {
            if(
                value is not AccessKind.NotSet 
                && (value is not AccessKind.Key || _entity.InitializingProperties)
                && value <= _propertyAccess
            )
            {
                _propertyAccess = value;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
    public bool IsSent 
    { 
        get => _isSent; 
        set
        {
            if (!value)
            {
                throw new InvalidOperationException();
            }
            if (!_isSent && value)
            {
                _isSent = true;
            }
        }
    }
    public EntityProperty(PocotaEntity entity)
    {
        _entity = entity;
    }
}
