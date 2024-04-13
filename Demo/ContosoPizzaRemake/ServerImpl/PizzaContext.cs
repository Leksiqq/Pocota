using ContosoPizza;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza;

public class PizzaContextImpl : PizzaContext
{
    public PizzaContextImpl(DbContextOptions<PizzaContext> options) : base(options)
    {
    }
}
