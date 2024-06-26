/////////////////////////////////////////////////////////////
// ContosoPizza.Models.ToppingAccessBase                   //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-03T16:59:17.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Net.Leksi.Pocota.Contract;
using Net.Leksi.Pocota.Server;
using System.Collections.Generic;

namespace ContosoPizza.Models;


public class ToppingAccessBase: IAccessCalculator
{
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    private readonly PizzaDbContext _dbContext;
    public ToppingAccessBase(IServiceProvider services)
    {
        _services = services;
        _context = _services.GetRequiredService<PocotaContext>();
        _dbContext = _services.GetRequiredService<PizzaDbContext>();
    }
    public AccessKind Calculate(object entity)
    {
        ToppingPocotaEntity pocotaEntity = _context.Entity<ToppingPocotaEntity>(entity);
        if(!pocotaEntity.IsAccessCalculated && entity is Topping value)
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
                        case "Pizzas":
                            if(value.Pizzas is {} && value.Pizzas.Count > 0)
                            {
                                IAccessCalculator accessCalculator = _services.GetRequiredKeyedService<IAccessCalculator>(typeof(Pizza));
                                foreach(Pizza item in value.Pizzas)
                                {
                                    AccessKind access = accessCalculator.Calculate(item);
                                    if(pocotaEntity.Access is AccessKind.Forbidden && access is AccessKind.Anonym)
                                    {
                                        pocotaEntity.Access = AccessKind.Anonym;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        return pocotaEntity.Access;
    }

    protected virtual void DoCalculate(Topping entity, ToppingPocotaEntity pocotaEntity) { }
}