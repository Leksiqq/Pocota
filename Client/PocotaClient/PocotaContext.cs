using Net.Leksi.Pocota.Contract;
using System.Reflection;

namespace Net.Leksi.Pocota.Client;

public class PocotaContext
{
    protected static readonly Dictionary<Type, Func<ulong, PocotaContext, PocotaEntity>> s_entityCreators = [];
    private readonly Dictionary<Type, Dictionary<string, bool>> _keyProperties = [];
    private readonly HashSet<Type> _keyPropertiesFilled = [];
    private readonly HashSet<ulong> _sentEntities = [];
    private PocotaConfig? _pocotaConfig;
    protected readonly IServiceProvider _services;
    private ulong _idGen = 0;
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
    public PocotaContext(IServiceProvider services)
    {
        _services = services;
    }
    public static T? Entity<T>(PocotaEntity entity) where T : class, IPocotaEntity
    {
        if(s_entityCreators.ContainsKey(typeof(T)))
        {
            return entity as T;
        }
        return null;
    }
    public static bool IsEntityType(Type type)
    {
        return s_entityCreators.ContainsKey(type);
    }
    public virtual T CreateEntity<T>() where T : PocotaEntity
    {
        return (T)s_entityCreators[typeof(T)].Invoke(Interlocked.Increment(ref _idGen), this);
    }
    public virtual object? CreateEntity(Type type)
    {
        return s_entityCreators.TryGetValue(type, out Func<ulong, PocotaContext, PocotaEntity>? creator) 
            ? creator.Invoke(Interlocked.Increment(ref _idGen), this) : null;
    }
    public bool IsKey(EntityProperty entityProperty)
    {
        return _keyProperties.TryGetValue(entityProperty.Entity.GetType(), out Dictionary<string, bool>? keys) && keys.ContainsKey(entityProperty.Name);
    }
    public bool? IsAutoKey(EntityProperty entityProperty)
    {
        return !_keyProperties.TryGetValue(entityProperty.Entity.GetType(), out Dictionary<string, bool>? keys) 
            || !keys.TryGetValue(entityProperty.Name, out bool ans) 
            ? null 
            : ans;
    }
    protected void ClearSentEntities()
    {
        _sentEntities.Clear();
    }
    protected bool IsSent(PocotaEntity entity)
    {
        return _sentEntities.Contains(((IPocotaEntity)entity).PocotaId);
    }
    protected bool SetSent(PocotaEntity entity)
    {
        return _sentEntities.Add(((IPocotaEntity)entity).PocotaId);
    }
    protected bool KeysFilled(PocotaEntity entity)
    {
        return _keyPropertiesFilled.Contains(entity.GetType());
    }
    protected void SetKeysFilled(PocotaEntity entity)
    {
        _keyPropertiesFilled.Add(entity.GetType());
    }
    protected static void SetPropertyState(EntityProperty property, PropertyState state)
    {
        property.State = state;
    }
}
