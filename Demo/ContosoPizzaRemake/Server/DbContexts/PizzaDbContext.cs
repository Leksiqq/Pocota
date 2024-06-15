/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaDbContext                             //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-15T08:59:47.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza;


public class PizzaDbContext: DbContext
{
    public PizzaDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Pizza> SetOfPizza => Set<Pizza>();
    public DbSet<Sauce> SetOfSauce => Set<Sauce>();
    public DbSet<Topping> SetOfTopping => Set<Topping>();
}
