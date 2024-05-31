using Net.Leksi.Pocota.Contract;
using System.Reflection;

namespace Net.Leksi.Pocota.Client;

public class PocotaContext
{
    private readonly Dictionary<Type, Dictionary<string, bool>> _keyProperties = [];
    private readonly HashSet<Type> _keyPropertiesFilled = [];
    private readonly HashSet<ulong> _sentEntities = [];
    private PocotaConfig? _pocotaConfig;
    private ulong _idGen = 0;
    protected readonly Dictionary<Type, Func<ulong, PocotaContext, IEntityOwner>> s_entityCreators = [];
    protected readonly IServiceProvider _services;
    public string ServiceKey { get; private init; }
    public bool KeyOnlyJson { get; internal set; } = true;
    internal PocotaConfig? PocotaConfig 
    { 
        get => _pocotaConfig;
        set
        {
            if(_pocotaConfig != value && value is { })
            {
                _pocotaConfig = value;
                _keyProperties.Clear();
                foreach(Type type in s_entityCreators.Keys)
                {
                    if(_pocotaConfig.Keys.TryGetValue(type.FullName!, out Dictionary<string, bool>? keys))
                    {
                        _keyProperties.Add(type, keys);
                    }
                }
            }
        }
    }
    public PocotaContext(IServiceProvider services, string serviceKey)
    {
        _services = services;
        ServiceKey = serviceKey;
    }
    public T CreateEntity<T>() where T : PocotaEntity
    {
        return (T)s_entityCreators[typeof(T)].Invoke(Interlocked.Increment(ref _idGen), this);
    }
    public IEntityOwner? CreateEntity(Type type)
    {
        return s_entityCreators.TryGetValue(type, out Func<ulong, PocotaContext, IEntityOwner>? creator) 
            ? creator.Invoke(Interlocked.Increment(ref _idGen), this) : null;
    }
    public bool IsKey(EntityProperty entityProperty)
    {
        return _keyProperties.TryGetValue(entityProperty.Entity.GetType(), out Dictionary<string, bool>? keys) && keys.ContainsKey(entityProperty.Name);
    }
    public bool KeysFilled(IPocotaEntity entity)
    {
        return _keyPropertiesFilled.Contains(entity.GetType());
    }
    internal static void SetPropertyState(EntityProperty property, PropertyState state)
    {
        property.State = state;
    }
    internal static void SetPropertyAccess(EntityProperty property, AccessKind access)
    {
        property.Access = access;
    }
    internal void ClearSentEntities()
    {
        _sentEntities.Clear();
    }
    internal bool IsSent(IPocotaEntity entity)
    {
        return _sentEntities.Contains(entity.PocotaId);
    }
    internal bool SetSent(IPocotaEntity entity)
    {
        return _sentEntities.Add(entity.PocotaId);
    }
    internal void SetKeysFilled(IPocotaEntity entity)
    {
        _keyPropertiesFilled.Add(entity.GetType());
    }
}
