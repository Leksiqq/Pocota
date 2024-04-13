/////////////////////////////////////////////////////////////
// ContosoPizza.Models.ToppingPocota                       //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-13T13:50:47.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Server;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models;


public class ToppingPocota: PocotaEntity
{
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    public ToppingPocota(IServiceProvider services)
    {
        _services = services;
        _context = _services.GetRequiredService<PocotaContext>();
    }
    public EntityProperty Id { get; private init; } = new();
    public EntityProperty Name { get; private init; } = new();
    public EntityProperty Calories { get; private init; } = new();
    public EntityProperty Pizzas { get; private init; } = new();
}