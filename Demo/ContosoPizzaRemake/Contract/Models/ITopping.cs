using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ContosoPizza.Models;

public interface ITopping
{
    int Id { get; set; }
    [Required]
    [MaxLength(100)]
    string? Name { get; set; }
    decimal Calories { get; set; }
    [JsonIgnore]
    ICollection<IPizza>? Pizzas { get; set; }
}