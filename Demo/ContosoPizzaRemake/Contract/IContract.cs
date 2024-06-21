using ContosoPizza.Models;
using Net.Leksi.Pocota.Contract;

namespace ContosoPizza;

[PocotaContract("Pizza")]
[Entity(typeof(Pizza))]
[Entity(typeof(Sauce))]
[Entity(typeof(Topping))]
[Envelope(typeof(PizzaFilter))]
public interface IContract
{
    IEnumerable<Pizza> GetAllPizzas();
    IEnumerable<Pizza> FindPizzas(PizzaFilter filter, int stage, bool? sure);
    Pizza GetPizza(Pizza pizza);
    IEnumerable<Sauce> GetAllSauces(int limit);
    Sauce GetSauce(Sauce sauce);
    IEnumerable<Topping> GetAllToppings();
    Topping GetTopping(Topping topping);
}
