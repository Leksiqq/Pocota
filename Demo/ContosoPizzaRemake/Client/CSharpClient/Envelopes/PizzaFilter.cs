/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaFilter                         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-20T15:57:53.                                 //
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
}