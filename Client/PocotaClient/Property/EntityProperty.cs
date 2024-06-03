using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Client;

public abstract class EntityProperty(IPocotaEntity entity, string name, Type type) : Property(name, type)
{
    private AccessKind _access = AccessKind.Full;
    private bool _readOnly = false;
    public IPocotaEntity Entity { get; private init; } = entity;
    public PropertyState State { get; internal set; }
    public AccessKind Access 
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
            }
        }
    }
    public override bool IsReadonly 
    {
        get
        {
            if (Entity.State is EntityState.Detached || Entity.State is EntityState.Created)
            {
                return _readOnly;
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
            return _readOnly;
        }
    }
    public void SetReadonly(bool value)
    {
        if(_readOnly != value)
        {
            _readOnly = value;
            NotifyPropertyChanged();
        }
    }
}
