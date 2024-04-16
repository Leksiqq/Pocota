using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza;

public class PizzaContextImpl : PizzaDbContext
{
    public PizzaContextImpl(DbContextOptions<PizzaContextImpl> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Sauce>().HasKey(x => new { x.Id, x.Id1 });
        modelBuilder.Entity<Sauce>().Property(p => p.Id).ValueGeneratedOnAdd();
    }
}
