/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Topping                      //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-04T18:29:40.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Net.Leksi.Pocota.Client;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models.Client;


public class Topping: PocotaEntity, IToppingPocotaEntity
{
    private const string s_Id = "Id";
    private const string s_Name = "Name";
    private const string s_Calories = "Calories";
    private const string s_Pizzas = "Pizzas";
    private readonly EntityProperty _IdEntityProperty = new(typeof(Topping), s_Id);
    private readonly EntityProperty _NameEntityProperty = new(typeof(Topping), s_Name);
    private readonly EntityProperty _CaloriesEntityProperty = new(typeof(Topping), s_Calories);
    private readonly EntityProperty _PizzasEntityProperty = new(typeof(Topping), s_Pizzas);
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
