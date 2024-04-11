/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Sauce                               //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-11T18:57:55.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;


public partial class Sauce
{
    public Int32 Id { get; set; }
    [Required(AllowEmptyStrings=false)]
    [MaxLength(100)]
    public String? Name { get; set; }
    public Boolean IsVegan { get; set; }
}
