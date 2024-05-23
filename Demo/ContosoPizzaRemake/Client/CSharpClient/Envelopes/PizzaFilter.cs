/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaFilter                         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-23T20:59:30.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models.Client;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Client;


public class PizzaFilter
{
    public List<Topping>? Toppings { get; set; }
    public List<Sauce>? Sauces { get; set; }
    public String? NameRegex { get; set; }
    public List<String>? Tags { get; set; }
    public List<Decimal>? decs { get; set; }
}