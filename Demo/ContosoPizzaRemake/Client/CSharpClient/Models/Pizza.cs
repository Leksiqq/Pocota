/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Pizza                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-29T17:17:24.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Net.Leksi.Pocota.Client;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models.Client;


public class Pizza: PocotaEntity, IPizzaPocotaEntity
{
    private readonly EntityProperty _IdEntityProperty = new();
    private readonly EntityProperty _NameEntityProperty = new();
    private readonly EntityProperty _SauceEntityProperty = new();
    private readonly EntityProperty _ToppingsEntityProperty = new();
    public Int32 Id { get; set; }
    public String? Name { get; set; }
    public Sauce? Sauce { get; set; }
    public ICollection<Topping>? Toppings { get; set; }
    EntityProperty IPizzaPocotaEntity.Id => _IdEntityProperty;
    EntityProperty IPizzaPocotaEntity.Name => _NameEntityProperty;
    EntityProperty IPizzaPocotaEntity.Sauce => _SauceEntityProperty;
    EntityProperty IPizzaPocotaEntity.Toppings => _ToppingsEntityProperty;
    internal Pizza(ulong pocotaId): base(pocotaId) { }
}
