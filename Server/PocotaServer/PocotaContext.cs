namespace Net.Leksi.Pocota.Server;

public class PocotaContext
{
    private readonly Dictionary<object, PocotaEntity> _entityCache = new(ReferenceEqualityComparer.Instance);
    private ulong _idGen = 0;
    public PocotaEntity Entity(object entity)
    {
        if(!_entityCache.TryGetValue(entity, out PocotaEntity? value))
        {
            value = new PocotaEntity
            {
                PocotaId = Interlocked.Increment(ref _idGen)
            };
            _entityCache.Add(entity, value);
        }
        return value;
    }
}
