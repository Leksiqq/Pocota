namespace Net.Leksi.Pocota.Client;

public class PocotaContext
{
    protected static readonly Dictionary<Type, Func<ulong, PocotaEntity>> s_entityCreators = [];
    private readonly Dictionary<Type, HashSet<string>> _keyProperties = [];
    private readonly HashSet<Type> _keyPropertiesFilled = [];
    private readonly HashSet<ulong> _sentEntities = [];
    protected readonly IServiceProvider _services;
    private ulong _idGen = 0;
    public PocotaContext(IServiceProvider services)
    {
        _services = services;
    }
    public static T? Entity<T>(PocotaEntity entity) where T : class, IPocotaEntity
    {
        return entity as T; 
    }
    protected T CreateEntity<T>() where T : PocotaEntity
    {
        return (T)s_entityCreators[typeof(T)].Invoke(Interlocked.Increment(ref _idGen));
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
    protected bool IsKey(EntityProperty property)
    {
        return _keyPropertiesFilled.Contains(property.EntityType) 
            && _keyProperties.TryGetValue(property.EntityType, out HashSet<string>? keys) 
            && keys.Contains(property.Name)
        ;
    }
    protected void MarkAsKey(EntityProperty property)
    {
        if (!_keyPropertiesFilled.Contains(property.EntityType))
        {
            if(!_keyProperties.TryGetValue(property.EntityType, out HashSet<string>? keys))
            {
                keys = [];
                _keyProperties.Add(property.EntityType, keys);
            }
            if(!keys.Contains(property.Name))
            {
                keys.Add(property.Name);
            }
        }
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
