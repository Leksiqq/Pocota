/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Topping                      //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-14T12:28:25.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Net.Leksi.Pocota.Client;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models.Client;


public class Topping: PocotaEntity, IToppingPocotaEntity
{
    private class IdProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Topping)Entity)?.Id;
            set {
                if(Entity is {} && value is Int32 val && val != ((Topping)Entity).Id) 
                {
                    ((Topping)Entity).Id = val;
                    OnPropertyChanged();
                }
            }
        }
    }
    private class NameProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Topping)Entity)?.Name;
            set {
                if(Entity is {} && value is String val && val != ((Topping)Entity).Name) 
                {
                    ((Topping)Entity).Name = val;
                    OnPropertyChanged();
                }
            }
        }
    }
    private class CaloriesProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Topping)Entity)?.Calories;
            set {
                if(Entity is {} && value is Decimal val && val != ((Topping)Entity).Calories) 
                {
                    ((Topping)Entity).Calories = val;
                    OnPropertyChanged();
                }
            }
        }
    }
    private class PizzasProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Topping)Entity)?.Pizzas;
            set {
                if(Entity is {} && value is ICollection<Pizza> val && val != ((Topping)Entity).Pizzas) 
                {
                    ((Topping)Entity).Pizzas = val;
                    OnPropertyChanged();
                }
            }
        }
    }
    private const string s_Id = "Id";
    private const string s_Name = "Name";
    private const string s_Calories = "Calories";
    private const string s_Pizzas = "Pizzas";
    private readonly IdProperty _IdEntityProperty;
    private readonly NameProperty _NameEntityProperty;
    private readonly CaloriesProperty _CaloriesEntityProperty;
    private readonly PizzasProperty _PizzasEntityProperty;
    public Int32 Id { get; set; }
    public String? Name { get; set; }
    public Decimal Calories { get; set; }
    public ICollection<Pizza>? Pizzas { get; set; }
    EntityProperty IToppingPocotaEntity.Id => _IdEntityProperty;
    EntityProperty IToppingPocotaEntity.Name => _NameEntityProperty;
    EntityProperty IToppingPocotaEntity.Calories => _CaloriesEntityProperty;
    EntityProperty IToppingPocotaEntity.Pizzas => _PizzasEntityProperty;
    internal Topping(ulong pocotaId, PocotaContext context): base(pocotaId, context) 
    {
        _IdEntityProperty = new IdProperty(this, s_Id, typeof(Int32));
        _NameEntityProperty = new NameProperty(this, s_Name, typeof(String));
        _CaloriesEntityProperty = new CaloriesProperty(this, s_Calories, typeof(Decimal));
        _PizzasEntityProperty = new PizzasProperty(this, s_Pizzas, typeof(ICollection<Pizza>));
    }
    protected override IEnumerable<EntityProperty> GetProperties()
    {
        yield return _IdEntityProperty;
        yield return _NameEntityProperty;
        yield return _CaloriesEntityProperty;
        yield return _PizzasEntityProperty;
    }
}
