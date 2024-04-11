/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaContext                               //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-11T18:57:55.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza;


public partial class PizzaContext: DbContext
{
    public PizzaContext(DbContextOptions<PizzaContext> options) : base(options) { }

    public DbSet<Pizza> SetOfPizza => Set<Pizza>();
    public DbSet<Sauce> SetOfSauce => Set<Sauce>();
    public DbSet<Topping> SetOfTopping => Set<Topping>();
}
