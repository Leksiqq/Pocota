

using Net.Leksi.Pocota.Contract;
using System.ComponentModel;

namespace Net.Leksi.Pocota.Client;

public abstract class PocotaEntity: IPocotaEntity, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static PropertyChangedEventArgs _accessChangedEventArgs = new(nameof(Access));
    private static PropertyChangedEventArgs _stateChangedEventArgs = new(nameof(State));
    private readonly object _owner;
    private AccessKind _access = AccessKind.Full;
    private EntityState _state = EntityState.Detached;
    protected readonly PocotaContext _context;
    protected readonly Dictionary<string, EntityProperty> _properties = [];
    public ulong PocotaId {  get; private init; }
    public EntityState State 
    { 
        get => _state;
        internal set
        {
            if (_state != value)
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
                if (value is AccessKind.Full || value is AccessKind.Readonly || value is AccessKind.Anonym)
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
    public PocotaEntity(ulong pocotaId, PocotaContext context, object owner)
    {
        PocotaId = pocotaId;
        _context = context;
        _owner = owner;
    }

    public EntityProperty? GetEntityProperty(string propertyName)
    {
        if(_properties.TryGetValue(propertyName, out EntityProperty? entityProperty))
        {
            return entityProperty;
        }
        return null;
    }
}
