using ContosoPizza.Models;
using Net.Leksi.Pocota.Contract;

namespace ContosoPizza;

[PocotaContract("Pizza")]
[Entity(typeof(IPizza))]
[Entity(typeof(ISauce))]
[Entity(typeof(ITopping))]
public interface IContract
{
    IEnumerable<IPizza> GetAllPizzas();
    IEnumerable<IPizza> FindPizzas(PizzaFilter filter);
    IPizza GetPizza(IPizza pizza);
    IEnumerable<ISauce> GetAllSauces();
    ISauce GetSauce(ISauce sauce);
    IEnumerable<ITopping> GetAllToppings();
    ITopping GetTopping(ITopping topping);
}
