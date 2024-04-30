/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaServiceBase                           //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-30T14:05:55.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContosoPizza;


public abstract class PizzaServiceBase
{
    public abstract IAsyncEnumerable<Pizza> GetAllPizzasAsync();
    public abstract IAsyncEnumerable<Pizza> FindPizzasAsync(PizzaFilter filter);
    public abstract ValueTask<Pizza?> GetPizzaAsync(Pizza pizza);
    public abstract IAsyncEnumerable<Sauce> GetAllSaucesAsync();
    public abstract ValueTask<Sauce?> GetSauceAsync(Sauce sauce);
    public abstract IAsyncEnumerable<Topping> GetAllToppingsAsync();
    public abstract ValueTask<Topping?> GetToppingAsync(Topping topping);
}
