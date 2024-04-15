/////////////////////////////////////////////////////////////
// ContosoPizza.Models.PizzaAccessBase                     //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-15T18:39:17.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Net.Leksi.Pocota.Server;
using System.Collections.Generic;

namespace ContosoPizza.Models;


public class PizzaAccessBase: IAccessCalculator
{
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    private readonly PizzaDbContext _dbContext;
    public PizzaAccessBase(IServiceProvider services)
    {
        _services = services;
        _context = _services.GetRequiredService<PocotaContext>();
        _dbContext = _services.GetRequiredService<PizzaDbContext>();
    }
    public void Calculate(object entity)
    {
        PizzaPocotaEntity pocotaEntity = _context.Entity<PizzaPocotaEntity>(entity);
        if(!pocotaEntity.IsAccessCalculated && entity is Pizza value)
        {
            pocotaEntity.IsAccessCalculated = true;
            DoCalculate(value, pocotaEntity);
            EntityEntry entityEntry = _dbContext.Entry(entity);
            foreach (ReferenceEntry entry in entityEntry.References)
            {
                if (entry.IsLoaded)
                {
                    switch (entry.Metadata.Name)
                    {
                        case "Sauce":
                            if(value.Sauce is {})
                            {
                                IAccessCalculator accessCalculator = _services.GetRequiredKeyedService<IAccessCalculator>(typeof(Sauce));
                                accessCalculator.Calculate(value.Sauce);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            foreach (CollectionEntry entry in entityEntry.Collections)
            {
                if (entry.IsLoaded)
                {
                    switch (entry.Metadata.Name)
                    {
                        case "Toppings":
                            if(value.Toppings is {} && value.Toppings.Count > 0)
                            {
                                IAccessCalculator accessCalculator = _services.GetRequiredKeyedService<IAccessCalculator>(typeof(Topping));
                                foreach(Topping item in value.Toppings)
                                {
                                    accessCalculator.Calculate(item);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    protected virtual void DoCalculate(Pizza entity, PizzaPocotaEntity pocotaEntity) { }
}