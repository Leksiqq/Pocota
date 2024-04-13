/////////////////////////////////////////////////////////////
// ContosoPizza.PizzaContextBase                           //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-13T12:19:10.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza;


public class PizzaContextBase: DbContext
{
    public PizzaContextBase(DbContextOptions<PizzaContextBase> options) : base(options) { }

    public DbSet<Pizza> SetOfPizza => Set<Pizza>();
    public DbSet<Sauce> SetOfSauce => Set<Sauce>();
    public DbSet<Topping> SetOfTopping => Set<Topping>();
}
