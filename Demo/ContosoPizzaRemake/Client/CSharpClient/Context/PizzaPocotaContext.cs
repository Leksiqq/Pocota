/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaPocotaContext                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-30T18:11:42.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models.Client;
using Net.Leksi.Pocota.Client;
using System.Collections.Generic;

namespace ContosoPizza.Client;


public class PizzaPocotaContext: PocotaContext
{
    public PizzaPocotaContext(IServiceProvider services): base(services) 
    {
        s_entityCreators.Add(typeof(Pizza), (id, ctx) => new Pizza(id, ctx));
        s_entityCreators.Add(typeof(Sauce), (id, ctx) => new Sauce(id, ctx));
        s_entityCreators.Add(typeof(Topping), (id, ctx) => new Topping(id, ctx));
    }
    internal static void Touch() { }
}