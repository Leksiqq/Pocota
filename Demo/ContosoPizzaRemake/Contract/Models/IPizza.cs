using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public interface IPizza
{
    [Key]
    int Id { get; set; }
    [Required]
    [MaxLength(100)]
    string? Name { get; set; }

    ISauce? Sauce { get; set; }

    ICollection<ITopping>? Toppings { get; set; }
}
