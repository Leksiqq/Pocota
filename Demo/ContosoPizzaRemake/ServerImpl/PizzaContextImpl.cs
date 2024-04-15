using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza;

public class PizzaContextImpl : PizzaDbContext
{
    public PizzaContextImpl(DbContextOptions<PizzaDbContext> options) : base(options)
    {
    }
}
