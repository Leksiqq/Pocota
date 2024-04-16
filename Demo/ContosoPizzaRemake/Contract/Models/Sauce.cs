using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoPizza.Models;

public class Sauce
{
    public int Id { get; set; }
    public int Id1 { get; set; }
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }
    public bool IsVegan { get; set; }
}