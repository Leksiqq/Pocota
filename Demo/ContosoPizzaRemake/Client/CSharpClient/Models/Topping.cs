/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Topping                      //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-21T10:35:18.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Net.Leksi.Pocota.Client;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models.Client;


public class Topping: PocotaEntity
{
    private class IdProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Topping)Entity)?.Id;
            set {
                if(Entity is {}) 
                {
                    if(value is Int32 val && val != ((Topping)Entity).Id)
                    {
                        ((Topping)Entity).Id = val;
                        NotifyPropertyChanged();
                    }
                    else if(value == default && ((Topping)Entity).Id != default)
                    {
                        ((Topping)Entity).Id = default;
                        NotifyPropertyChanged();
                    }
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
                if(Entity is {}) 
                {
                    if(value is String val && val != ((Topping)Entity).Name)
                    {
                        ((Topping)Entity).Name = val;
                        NotifyPropertyChanged();
                    }
                    else if(value == default && ((Topping)Entity).Name != default)
                    {
                        ((Topping)Entity).Name = default;
                        NotifyPropertyChanged();
                    }
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
                if(Entity is {}) 
                {
                    if(value is Decimal val && val != ((Topping)Entity).Calories)
                    {
                        ((Topping)Entity).Calories = val;
                        NotifyPropertyChanged();
                    }
                    else if(value == default && ((Topping)Entity).Calories != default)
                    {
                        ((Topping)Entity).Calories = default;
                        NotifyPropertyChanged();
                    }
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
                if(Entity is {}) 
                {
                    if(value is ICollection<Pizza> val && val != ((Topping)Entity).Pizzas)
                    {
                        ((Topping)Entity).Pizzas = val;
                        NotifyPropertyChanged();
                    }
                    else if(value == default && ((Topping)Entity).Pizzas != default)
                    {
                        ((Topping)Entity).Pizzas = default;
                        NotifyPropertyChanged();
                    }
                }
            }
        }
    }
    private class ToppingPocotaEntity(Topping owner) : IToppingPocotaEntity
    {
        public EntityProperty Id => owner._IdEntityProperty;
        public EntityProperty Name => owner._NameEntityProperty;
        public EntityProperty Calories => owner._CaloriesEntityProperty;
        public EntityProperty Pizzas => owner._PizzasEntityProperty;
        public ulong PocotaId => ((IPocotaEntity)owner).PocotaId;

        public EntityState State => ((IPocotaEntity)owner).State;

        public IEnumerable<EntityProperty> Properties => ((IPocotaEntity)owner).Properties;
        public IPocotaEntity Entity => this;
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
    internal Topping(ulong pocotaId, PocotaContext context): base(pocotaId, context) 
    {
        _IdEntityProperty = new IdProperty(this, s_Id, typeof(Int32));
        _NameEntityProperty = new NameProperty(this, s_Name, typeof(String));
        _CaloriesEntityProperty = new CaloriesProperty(this, s_Calories, typeof(Decimal));
        _PizzasEntityProperty = new PizzasProperty(this, s_Pizzas, typeof(ICollection<Pizza>));
        _entity = new ToppingPocotaEntity(this);
    }
    protected override IEnumerable<EntityProperty> GetProperties()
    {
        yield return _IdEntityProperty;
        yield return _NameEntityProperty;
        yield return _CaloriesEntityProperty;
        yield return _PizzasEntityProperty;
    }
}
