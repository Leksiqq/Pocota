using ContosoPizza.Models;

namespace ContosoPizza;

public class PizzaFilter
{
    public List<Topping>? Toppings { get; set; }
    public List<Sauce>? Sauces { get; set; }
    public string? NameRegex { get; set; }
    public List<string>? Tags { get; set; }
    public List<decimal>? decs { get; set; }
}