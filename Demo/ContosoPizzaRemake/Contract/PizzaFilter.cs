using ContosoPizza.Models;

namespace ContosoPizza;

public class PizzaFilter
{
    public List<ITopping> Toppings { get; private init; } = [];
    public List<ISauce> Sauces { get; private init; } = [];
    public string? NameRegex { get; set; }
}