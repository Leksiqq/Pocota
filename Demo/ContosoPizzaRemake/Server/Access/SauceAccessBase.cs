/////////////////////////////////////////////////////////////
// ContosoPizza.Models.SauceAccessBase                     //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-17T16:15:52.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Net.Leksi.Pocota.Contract;
using Net.Leksi.Pocota.Server;

namespace ContosoPizza.Models;


public class SauceAccessBase: IAccessCalculator
{
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    private readonly PizzaDbContext _dbContext;
    public SauceAccessBase(IServiceProvider services)
    {
        _services = services;
        _context = _services.GetRequiredService<PocotaContext>();
        _dbContext = _services.GetRequiredService<PizzaDbContext>();
    }
    public AccessKind Calculate(object entity)
    {
        SaucePocotaEntity pocotaEntity = _context.Entity<SaucePocotaEntity>(entity);
        if(!pocotaEntity.IsAccessCalculated && entity is Sauce value)
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
                        default:
                            break;
                    }
                }
            }
        }
        return pocotaEntity.Access;
    }

    protected virtual void DoCalculate(Sauce entity, SaucePocotaEntity pocotaEntity) { }
}