/////////////////////////////////////////////////////////////
// ContosoPizza.Models.PizzaPocota                         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-14T15:28:52.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Net.Leksi.Pocota.Server;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models;


public class PizzaPocota: PocotaEntity
{
    private readonly IServiceProvider _services;
    private readonly PizzaContext _dbContext;
    private readonly PocotaContext _context;
    private EntityProperty _Id = null!;
    private EntityProperty _Name = null!;
    private EntityProperty _Sauce = null!;
    private EntityProperty _Toppings = null!;
    public PizzaPocota(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaContext>();
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
                    _Id ??= new EntityProperty(property);
                    break;
                case "Name":
                    _Name ??= new EntityProperty(property);
                    break;
                case "Sauce":
                    _Sauce ??= new EntityProperty(property);
                    break;
                case "Toppings":
                    _Toppings ??= new EntityProperty(property);
                    break;
            }
        }
        foreach (NavigationEntry navigation in entityEntry.Navigations)
        {
            switch(navigation.Metadata.Name)
            {
                case "Id":
                    _Id ??= new EntityProperty(navigation);
                    break;
                case "Name":
                    _Name ??= new EntityProperty(navigation);
                    break;
                case "Sauce":
                    _Sauce ??= new EntityProperty(navigation);
                    break;
                case "Toppings":
                    _Toppings ??= new EntityProperty(navigation);
                    break;
            }
        }
    }
}