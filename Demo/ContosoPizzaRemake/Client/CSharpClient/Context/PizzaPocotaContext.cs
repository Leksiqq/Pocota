/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaPocotaContext                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-29T15:06:27.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models.Client;
using Net.Leksi.Pocota.Client;
using System.Collections.Generic;

namespace ContosoPizza.Client;


public class PizzaPocotaContext: PocotaContext
{
    private static readonly Dictionary<Type, Func<ulong,PocotaEntity>> _entityCreators = new()
    {
        {typeof(Pizza), id => new Pizza(id)},
        {typeof(Sauce), id => new Sauce(id)},
        {typeof(Topping), id => new Topping(id)},
    };
    private readonly HashSet<ulong> _sentEntities = new();
    public PizzaPocotaContext(IServiceProvider services): base(services) { }
    internal T CreateEntity<T>() where T : PocotaEntity
    {
        return (T)_entityCreators[typeof(T)].Invoke(Interlocked.Increment(ref _idGen));
    }
    internal void ClearSentEntities() {
        _sentEntities.Clear();
    }
    internal bool IsSent(PocotaEntity entity) 
    {
        return _sentEntities.Contains(((IPocotaEntity)entity).PocotaId);
    }
    internal bool SetSent(PocotaEntity entity)
    {
        return _sentEntities.Add(((IPocotaEntity)entity).PocotaId);
    }
}