/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaPocotaContext                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-23T20:59:30.                                 //
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
        s_entityCreators.Add(typeof(Pizza), (id, ctx) => new Pizza(id, ctx));
        s_entityCreators.Add(typeof(Sauce), (id, ctx) => new Sauce(id, ctx));
        s_entityCreators.Add(typeof(Topping), (id, ctx) => new Topping(id, ctx));
    }
    public PizzaPocotaContext(IServiceProvider services): base(services) { }
    internal bool KeyOnlyJson {  get; set; }
    public override T CreateEntity<T>()
    {
        return base.CreateEntity<T>();
    }
    internal static void Touch() { }
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
    internal new bool KeysFilled(PocotaEntity entity)
    {
        return base.KeysFilled(entity);
    }
    internal new void SetKeysFilled(PocotaEntity entity)
    {
        base.SetKeysFilled(entity);
    }
}