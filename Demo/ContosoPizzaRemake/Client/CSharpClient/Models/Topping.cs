/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Topping                      //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-29T15:06:28.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Net.Leksi.Pocota.Client;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models.Client;


public class Topping: PocotaEntity, IToppingPocotaEntity
{
    private readonly EntityProperty _IdEntityProperty = new();
    private readonly EntityProperty _NameEntityProperty = new();
    private readonly EntityProperty _CaloriesEntityProperty = new();
    private readonly EntityProperty _PizzasEntityProperty = new();
    public Int32 Id { get; set; }
    public String? Name { get; set; }
    public Decimal Calories { get; set; }
    public ICollection<Pizza>? Pizzas { get; set; }
    EntityProperty IToppingPocotaEntity.Id => _IdEntityProperty;
    EntityProperty IToppingPocotaEntity.Name => _NameEntityProperty;
    EntityProperty IToppingPocotaEntity.Calories => _CaloriesEntityProperty;
    EntityProperty IToppingPocotaEntity.Pizzas => _PizzasEntityProperty;
    internal Topping(ulong pocotaId): base(pocotaId) { }
}
