using Net.Leksi.Pocota.Contract;
using System.ComponentModel;

namespace Net.Leksi.Pocota.Client;

public class EntityProperty(IPocotaEntity entity, string name, Type type): INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _stateChangedEventArgs = new(nameof(State));
    private static readonly PropertyChangedEventArgs _accessChangedEventArgs = new(nameof(Access));
    private static readonly PropertyChangedEventArgs _isReadonlyChangedEventArgs = new(nameof(Access));
    private bool _isSetReadonly = false;
    private AccessKind _access = AccessKind.Full;
    private PropertyState _state = PropertyState.Unchanged;
    public string Name => name;
    public Type Type => type;
    public IPocotaEntity Entity => entity;
    public PropertyState State 
    {
        get => _state;  
        internal set
        {
            if(_state != value)
            {
                _state = value;
                PropertyChanged?.Invoke(this, _stateChangedEventArgs);
            }
        }
    }
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
                PropertyChanged?.Invoke(this, _accessChangedEventArgs);
            }
        }
    }
    public bool IsSetReadOnly
    {
        get => _isSetReadonly;
        set
        {
            if (_isSetReadonly != value)
            {
                _isSetReadonly = value;
                PropertyChanged?.Invoke(this, _isReadonlyChangedEventArgs);
            }
        }
    }
    public bool IsReadonly 
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
