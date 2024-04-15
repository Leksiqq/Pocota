using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Server;

public class EntityProperty
{
    private bool _isSent = false;
    protected PropertyAccess _propertyAccess = PropertyAccess.Full;
    private readonly PocotaEntity _entity;
    public virtual object? NotSetStub { get => null; }
    public virtual PropertyAccess Access 
    { 
        get => _propertyAccess; 
        set
        {
            if(
                value is not PropertyAccess.NotSet 
                && (value is not PropertyAccess.Key || _entity.InitializingProperties)
                && value < _propertyAccess
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
