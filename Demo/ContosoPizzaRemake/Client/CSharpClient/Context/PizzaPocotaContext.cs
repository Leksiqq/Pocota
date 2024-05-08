/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaPocotaContext                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-08T20:36:28.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models.Client;
using Net.Leksi.Pocota.Client;
using System.Collections.Generic;

namespace ContosoPizza.Client;


public class PizzaPocotaContext: PocotaContext
{
    static PizzaPocotaContext()
    {
        PocotaContext.s_entityCreators.Add(typeof(Pizza), id => new Pizza(id));
        PocotaContext.s_entityCreators.Add(typeof(Sauce), id => new Sauce(id));
        PocotaContext.s_entityCreators.Add(typeof(Topping), id => new Topping(id));
    }
    public PizzaPocotaContext(IServiceProvider services): base(services) { }
    internal bool KeyOnlyJson {  get; set; }
    internal new T CreateEntity<T>() where T : PocotaEntity
    {
        return base.CreateEntity<T>();
    }
    internal new void ClearSentEntities() {
        base.ClearSentEntities();
    }
    internal new bool IsSent(PocotaEntity entity) 
    {
        return base.IsSent(entity);
    }
    internal new bool SetSent(PocotaEntity entity)
    {
        return base.SetSent(entity);
    }
    internal new bool IsKey(EntityProperty property) 
    {
        return base.IsKey(property);
    }
    internal new void MarkAsKey(EntityProperty property)
    {
        base.MarkAsKey(property);
    }
    internal new bool KeysFilled(PocotaEntity entity)
    {
        return base.KeysFilled(entity);
    }
    internal new void SetKeysFilled(PocotaEntity entity)
    {
        base.SetKeysFilled(entity);
    }
}