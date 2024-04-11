/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Topping                             //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-11T18:57:55.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models;


public partial class Topping
{
    public Int32 Id { get; set; }
    [Required(AllowEmptyStrings=false)]
    [MaxLength(100)]
    public String? Name { get; set; }
    public Decimal Calories { get; set; }
    [JsonIgnore(Condition=JsonIgnoreCondition.Always)]
    public ICollection<Pizza>? Pizzas { get; set; }
}
