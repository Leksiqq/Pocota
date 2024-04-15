using Microsoft.Extensions.DependencyInjection;

namespace Net.Leksi.Pocota.Server;

public class PocotaContext
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<object, PocotaEntity> _entityCache = new(ReferenceEqualityComparer.Instance);
    private ulong _idGen = 0;
    public PocotaContext(IServiceProvider services)
    {
        _services = services;
    }

    public T Entity<T>(object entity) where T : PocotaEntity
    {
        if(!_entityCache.TryGetValue(entity, out PocotaEntity? value))
        {
            value = _services.GetRequiredService<T>();
            value.PocotaId = Interlocked.Increment(ref _idGen);
            value.Entity = entity;
            _entityCache.Add(entity, value);
        }
        return (T)value;
    }
    public static async IAsyncEnumerable<TEntity> ProcessEntitiesAsync<TEntity>(IAccessCalculator accessCalculator, IAsyncEnumerable<TEntity> entities)
    { 
        await foreach(TEntity entity in entities)
        {
            accessCalculator.Calculate(entity!);
            yield return entity;
        }
    }

    public static TEntity ProcessEntity<TEntity>(IAccessCalculator accessCalculator, TEntity entity)
    {
        accessCalculator.Calculate(entity!);
        return entity;
    }
}
