/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaServiceBase                           //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-21T16:41:59.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContosoPizza;


public abstract class PizzaServiceBase
{
    public abstract IAsyncEnumerable<Pizza> GetAllPizzasAsync();
    public abstract IAsyncEnumerable<Pizza> FindPizzasAsync(PizzaFilter filter, Int32 stage, Boolean? sure);
    public abstract ValueTask<Pizza?> GetPizzaAsync(Pizza pizza);
    public abstract IAsyncEnumerable<Sauce> GetAllSaucesAsync(Int32 limit);
    public abstract ValueTask<Sauce?> GetSauceAsync(Sauce sauce);
    public abstract IAsyncEnumerable<Topping> GetAllToppingsAsync();
    public abstract ValueTask<Topping?> GetToppingAsync(Topping topping);
}
