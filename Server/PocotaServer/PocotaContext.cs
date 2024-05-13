using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Contract;

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
    public PocotaConfig GetPocotaConfig(DbContext dbContext, HashSet<Type> entities)
    {
        PocotaConfig config = new();
        foreach (Type type in entities)
        {
            config.Keys.Add(type.FullName!, []);
            object obj = Activator.CreateInstance(type)!;
            foreach (var prop in dbContext.Entry(obj).Properties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    config.Keys[type.FullName!].Add(prop.Metadata.Name, prop.Metadata.ValueGenerated is not ValueGenerated.Never);
                }
            }
        }
        return config;
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
