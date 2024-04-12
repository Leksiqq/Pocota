using ContosoPizza;
using Microsoft.EntityFrameworkCore;

namespace ServerImpl;

public class PizzaContextImpl : PizzaContext
{
    public PizzaContextImpl(DbContextOptions<PizzaContext> options) : base(options)
    {
    }
}
