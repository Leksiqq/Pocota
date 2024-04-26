/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Topping                             //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-26T12:56:14.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ContosoPizza.Models;


public class Topping
{
    public Int32 Id { get; set; }
    public String? Name { get; set; }
    public Decimal Calories { get; set; }
    public ICollection<Pizza>? Pizzas { get; set; }
}
