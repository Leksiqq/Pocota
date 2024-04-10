/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Sauce                               //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-10T18:10:30.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;


public class Sauce: ISauce
{
    public Int32 Id { get; set; }
    [Required(AllowEmptyStrings=false)]
    [MaxLength(100)]
    public String? Name { get; set; }
    public Boolean IsVegan { get; set; }
}
