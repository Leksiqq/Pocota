using ContosoPizza.Models;
using Net.Leksi.Pocota.Contract;

namespace ContosoPizza;

public class PizzaFilter
{
    public ICollection<Topping>? Toppings { get; set; }
    public ICollection<Sauce>? Sauces { get; set; }
    public string? NameRegex { get; set; }
    public ICollection<string>? Tags { get; set; }
    public ICollection<decimal>? Decs { get; set; }
    public DateTime? DateTime { get; set; }
    public DateOnly? DateOnly { get; set; }
    public TimeSpan? TimeSpan { get; set; }
    public AccessKind? AccessKind { get; set; }
    public bool? CanSing { get; set; }
    public ICollection<bool>? Bools { get; set; }
    public int NotNullInt { get; set; }
    public double DoubleValue { get; set; }
}