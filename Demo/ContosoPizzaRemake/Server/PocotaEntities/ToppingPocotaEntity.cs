/////////////////////////////////////////////////////////////
// ContosoPizza.Models.ToppingPocotaEntity                 //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-03T15:47:14.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Net.Leksi.Pocota.Server;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models;


public class ToppingPocotaEntity: PocotaEntity
{
    private readonly IServiceProvider _services;
    private readonly PizzaDbContext _dbContext;
    private readonly PocotaContext _context;
    private EntityProperty _Id = null!;
    private EntityProperty _Name = null!;
    private EntityProperty _Calories = null!;
    private EntityProperty _Pizzas = null!;
    public ToppingPocotaEntity(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaDbContext>();
        _context = _services.GetRequiredService<PocotaContext>();
    }
    public EntityProperty Id => _Id;
    public EntityProperty Name => _Name;
    public EntityProperty Calories => _Calories;
    public EntityProperty Pizzas => _Pizzas;
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
                case "Calories":
                    _Calories ??= new EntityPropertyProperty(this, property);
                    break;
                case "Pizzas":
                    _Pizzas ??= new EntityPropertyProperty(this, property);
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
                case "Calories":
                    _Calories ??= new EntityPropertyNavigation(this, navigation);
                    break;
                case "Pizzas":
                    _Pizzas ??= new EntityPropertyNavigation(this, navigation);
                    break;
            }
        }
    }
}