/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaFilter                         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-31T16:57:58.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models.Client;
using Net.Leksi.Pocota.Contract;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Client;


public class PizzaFilter
{
    public List<Topping>? Toppings { get; set; }
    public List<Sauce>? Sauces { get; set; }
    public String? NameRegex { get; set; }
    public List<String>? Tags { get; set; }
    public List<Decimal>? Decs { get; set; }
    public DateTime? DateTime { get; set; }
    public DateOnly? DateOnly { get; set; }
    public TimeOnly? TimeOnly { get; set; }
    public AccessKind? AccessKind { get; set; }
    public Boolean? CanSing { get; set; }
    public List<Boolean>? Bools { get; set; }
}