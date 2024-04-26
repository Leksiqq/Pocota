/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Pizza                               //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-26T12:56:14.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ContosoPizza.Models;


public class Pizza
{
    public Int32 Id { get; set; }
    public String? Name { get; set; }
    public Sauce? Sauce { get; set; }
    public ICollection<Topping>? Toppings { get; set; }
}
