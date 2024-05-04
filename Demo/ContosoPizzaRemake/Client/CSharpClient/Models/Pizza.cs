/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Pizza                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-04T18:29:40.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Net.Leksi.Pocota.Client;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models.Client;


public class Pizza: PocotaEntity, IPizzaPocotaEntity
{
    private const string s_Id = "Id";
    private const string s_Name = "Name";
    private const string s_Sauce = "Sauce";
    private const string s_Toppings = "Toppings";
    private readonly EntityProperty _IdEntityProperty = new(typeof(Pizza), s_Id);
    private readonly EntityProperty _NameEntityProperty = new(typeof(Pizza), s_Name);
    private readonly EntityProperty _SauceEntityProperty = new(typeof(Pizza), s_Sauce);
    private readonly EntityProperty _ToppingsEntityProperty = new(typeof(Pizza), s_Toppings);
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
