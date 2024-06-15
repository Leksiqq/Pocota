using ContosoPizza.Models;
using Net.Leksi.Pocota.Contract;

namespace ContosoPizza;

public class PizzaFilter
{
    public List<Topping>? Toppings { get; set; }
    public List<Sauce>? Sauces { get; set; }
    public string? NameRegex { get; set; }
    public List<string>? Tags { get; set; }
    public List<decimal>? Decs { get; set; }
    public DateTime? DateTime { get; set; }
    public DateOnly? DateOnly { get; set; }
    public TimeSpan? TimeSpan { get; set; }
    public AccessKind? AccessKind { get; set; }
    public bool? CanSing { get; set; }
    public List<bool>? Bools { get; set; }
    public int NotNullInt { get; set; }
    public double DoubleValue { get; set; }
}