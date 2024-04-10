using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public interface ISauce
{
    int Id { get; set; }
    [Required]
    [MaxLength(100)]
    string? Name { get; set; }
    bool IsVegan { get; set; }
}
