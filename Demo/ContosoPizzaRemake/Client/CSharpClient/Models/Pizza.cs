/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Pizza                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-13T17:59:08.                                 //
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
    private readonly EntityProperty _IdEntityProperty;
    private readonly EntityProperty _NameEntityProperty;
    private readonly EntityProperty _SauceEntityProperty;
    private readonly EntityProperty _ToppingsEntityProperty;
    public Int32 Id { get; set; }
    public String? Name { get; set; }
    public Sauce? Sauce { get; set; }
    public ICollection<Topping>? Toppings { get; set; }
    EntityProperty IPizzaPocotaEntity.Id => _IdEntityProperty;
    EntityProperty IPizzaPocotaEntity.Name => _NameEntityProperty;
    EntityProperty IPizzaPocotaEntity.Sauce => _SauceEntityProperty;
    EntityProperty IPizzaPocotaEntity.Toppings => _ToppingsEntityProperty;
    internal Pizza(ulong pocotaId, PocotaContext context): base(pocotaId, context) 
    {
        _IdEntityProperty = new EntityProperty(this, s_Id);
        _NameEntityProperty = new EntityProperty(this, s_Name);
        _SauceEntityProperty = new EntityProperty(this, s_Sauce);
        _ToppingsEntityProperty = new EntityProperty(this, s_Toppings);
    }
}
