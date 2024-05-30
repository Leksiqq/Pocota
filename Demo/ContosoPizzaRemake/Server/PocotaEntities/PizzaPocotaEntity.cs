/////////////////////////////////////////////////////////////
// ContosoPizza.Models.PizzaPocotaEntity                   //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-30T18:11:42.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Net.Leksi.Pocota.Server;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models;


public class PizzaPocotaEntity: PocotaEntity
{
    private readonly IServiceProvider _services;
    private readonly PizzaDbContext _dbContext;
    private readonly PocotaContext _context;
    private EntityProperty _Id = null!;
    private EntityProperty _Name = null!;
    private EntityProperty _Sauce = null!;
    private EntityProperty _Toppings = null!;
    public PizzaPocotaEntity(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaDbContext>();
        _context = _services.GetRequiredService<PocotaContext>();
    }
    public EntityProperty Id => _Id;
    public EntityProperty Name => _Name;
    public EntityProperty Sauce => _Sauce;
    public EntityProperty Toppings => _Toppings;
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
                case "Name":
                    _Name ??= new EntityPropertyProperty(this, property);
                    break;
                case "Sauce":
                    _Sauce ??= new EntityPropertyProperty(this, property);
                    break;
                case "Toppings":
                    _Toppings ??= new EntityPropertyProperty(this, property);
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
                case "Name":
                    _Name ??= new EntityPropertyNavigation(this, navigation);
                    break;
                case "Sauce":
                    _Sauce ??= new EntityPropertyNavigation(this, navigation);
                    break;
                case "Toppings":
                    _Toppings ??= new EntityPropertyNavigation(this, navigation);
                    break;
            }
        }
    }
}