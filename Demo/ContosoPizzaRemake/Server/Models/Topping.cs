/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Topping                             //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-10T18:10:30.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models;


public class Topping: ITopping
{
    public Int32 Id { get; set; }
    [Required(AllowEmptyStrings=false)]
    [MaxLength(100)]
    public String? Name { get; set; }
    public Decimal Calories { get; set; }
    [JsonIgnore(Condition=Always)]
    public ICollection<IPizza>? Pizzas { get; set; }
}
