using ContosoPizza.Models;

namespace ContosoPizza;

public class PizzaFilter
{
    public List<Topping> Toppings { get; private init; } = [];
    public List<Sauce> Sauces { get; private init; } = [];
    public string? NameRegex { get; set; }
    public List<string>? Tags { get; set; } = ["foo", "bar"];
    public Dictionary<string, string>? Dict { get; set; }
}