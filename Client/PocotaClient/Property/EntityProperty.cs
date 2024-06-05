using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Client;

public abstract class EntityProperty(IPocotaEntity entity, string name, Type type) : Property(name, type)
{
    private AccessKind _access = AccessKind.Full;
    private PropertyState _state = PropertyState.Unchanged;
    public IPocotaEntity Entity { get; private init; } = entity;
    public override PropertyState State 
    {
        get => _state;  
        internal set
        {
            if(_state != value)
            {
                _state = value;
                NotifyPropertyChanged();
            }
        }
    }
    public override AccessKind Access 
    { 
        get => _access;
        internal set
        {
            if (_access != value)
            {
                if (value is not AccessKind.Anonym)
                {
                    _access = value;
                }
                else
                {
                    _access = AccessKind.Readonly;
                }
                NotifyPropertyChanged();
            }
        }
    }
    public override bool IsReadonly 
    {
        get
        {
            if (Entity.State is EntityState.Detached || Entity.State is EntityState.Created)
            {
                return IsSetReadOnly;
            }
            if (
                Entity.State is EntityState.Deleted
                || Entity.Access is AccessKind.Readonly
                || Entity.Access is AccessKind.Anonym
                || Access is AccessKind.NotSet
                || Access is AccessKind.Forbidden
                || Access is AccessKind.Full
                || Access is AccessKind.Readonly
            )
            {
                return true;
            }
            return IsSetReadOnly;
        }
    }
}
