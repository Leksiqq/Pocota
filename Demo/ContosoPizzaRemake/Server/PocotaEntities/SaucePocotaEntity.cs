/////////////////////////////////////////////////////////////
// ContosoPizza.Models.SaucePocotaEntity                   //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-15T10:42:54.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Net.Leksi.Pocota.Server;
using System;

namespace ContosoPizza.Models;


public class SaucePocotaEntity: PocotaEntity
{
    private readonly IServiceProvider _services;
    private readonly PizzaDbContext _dbContext;
    private readonly PocotaContext _context;
    private EntityProperty _Id = null!;
    private EntityProperty _Id1 = null!;
    private EntityProperty _Name = null!;
    private EntityProperty _IsVegan = null!;
    public SaucePocotaEntity(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaDbContext>();
        _context = _services.GetRequiredService<PocotaContext>();
    }
    public EntityProperty Id => _Id;
    public EntityProperty Id1 => _Id1;
    public EntityProperty Name => _Name;
    public EntityProperty IsVegan => _IsVegan;
    protected override void InitProperties()
    {
        EntityEntry entityEntry = _dbContext.Entry(Entity!);
        foreach (PropertyEntry property in entityEntry.Properties)
        {
            switch(property.Metadata.Name)
            {
                case "Id":
                    _Id ??= new EntityPropertyProperty(this, property);
                    break;
                case "Id1":
                    _Id1 ??= new EntityPropertyProperty(this, property);
                    break;
                case "Name":
                    _Name ??= new EntityPropertyProperty(this, property);
                    break;
                case "IsVegan":
                    _IsVegan ??= new EntityPropertyProperty(this, property);
                    break;
            }
        }
        foreach (NavigationEntry navigation in entityEntry.Navigations)
        {
            switch(navigation.Metadata.Name)
            {
                case "Id":
                    _Id ??= new EntityPropertyNavigation(this, navigation);
                    break;
                case "Id1":
                    _Id1 ??= new EntityPropertyNavigation(this, navigation);
                    break;
                case "Name":
                    _Name ??= new EntityPropertyNavigation(this, navigation);
                    break;
                case "IsVegan":
                    _IsVegan ??= new EntityPropertyNavigation(this, navigation);
                    break;
            }
        }
    }
}