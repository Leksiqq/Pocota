/////////////////////////////////////////////////////////////
// ContosoPizza.Models.ToppingPocota                       //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-14T15:28:52.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Net.Leksi.Pocota.Server;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models;


public class ToppingPocota: PocotaEntity
{
    private readonly IServiceProvider _services;
    private readonly PizzaContext _dbContext;
    private readonly PocotaContext _context;
    private EntityProperty _Id = null!;
    private EntityProperty _Name = null!;
    private EntityProperty _Calories = null!;
    private EntityProperty _Pizzas = null!;
    public ToppingPocota(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaContext>();
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
                    _Id ??= new EntityProperty(property);
                    break;
                case "Name":
                    _Name ??= new EntityProperty(property);
                    break;
                case "Calories":
                    _Calories ??= new EntityProperty(property);
                    break;
                case "Pizzas":
                    _Pizzas ??= new EntityProperty(property);
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
                case "Calories":
                    _Calories ??= new EntityProperty(navigation);
                    break;
                case "Pizzas":
                    _Pizzas ??= new EntityProperty(navigation);
                    break;
            }
        }
    }
}