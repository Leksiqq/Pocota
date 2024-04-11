/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Pizza                               //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-11T18:57:55.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;


public partial class Pizza
{
    [Key]
    public Int32 Id { get; set; }
    [Required(AllowEmptyStrings=false)]
    [MaxLength(100)]
    public String? Name { get; set; }
    public Sauce? Sauce { get; set; }
    public ICollection<Topping>? Toppings { get; set; }
}
