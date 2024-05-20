/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Pizza                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-20T17:10:07.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Net.Leksi.Pocota.Client;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models.Client;


public class Pizza: PocotaEntity
{
    private class IdProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Pizza)Entity)?.Id;
            set {
                if(Entity is {} && value is Int32 val && val != ((Pizza)Entity).Id) 
                {
                    ((Pizza)Entity).Id = val;
                    NotifyPropertyChanged();
                }
            }
        }
    }
    private class NameProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Pizza)Entity)?.Name;
            set {
                if(Entity is {} && value is String val && val != ((Pizza)Entity).Name) 
                {
                    ((Pizza)Entity).Name = val;
                    NotifyPropertyChanged();
                }
            }
        }
    }
    private class SauceProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Pizza)Entity)?.Sauce;
            set {
                if(Entity is {} && value is Sauce val && val != ((Pizza)Entity).Sauce) 
                {
                    ((Pizza)Entity).Sauce = val;
                    NotifyPropertyChanged();
                }
            }
        }
    }
    private class ToppingsProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Pizza)Entity)?.Toppings;
            set {
                if(Entity is {} && value is ICollection<Topping> val && val != ((Pizza)Entity).Toppings) 
                {
                    ((Pizza)Entity).Toppings = val;
                    NotifyPropertyChanged();
                }
            }
        }
    }
    private class PizzaPocotaEntity(Pizza owner) : IPizzaPocotaEntity
    {
        public EntityProperty Id => owner._IdEntityProperty;
        public EntityProperty Name => owner._NameEntityProperty;
        public EntityProperty Sauce => owner._SauceEntityProperty;
        public EntityProperty Toppings => owner._ToppingsEntityProperty;
        public ulong PocotaId => ((IPocotaEntity)owner).PocotaId;

        public EntityState State => ((IPocotaEntity)owner).State;

        public IEnumerable<EntityProperty> Properties => ((IPocotaEntity)owner).Properties;
        public IPocotaEntity Entity => this;
    }
    private const string s_Id = "Id";
    private const string s_Name = "Name";
    private const string s_Sauce = "Sauce";
    private const string s_Toppings = "Toppings";
    private readonly IdProperty _IdEntityProperty;
    private readonly NameProperty _NameEntityProperty;
    private readonly SauceProperty _SauceEntityProperty;
    private readonly ToppingsProperty _ToppingsEntityProperty;
    public Int32 Id { get; set; }
    public String? Name { get; set; }
    public Sauce? Sauce { get; set; }
    public ICollection<Topping>? Toppings { get; set; }
    internal Pizza(ulong pocotaId, PocotaContext context): base(pocotaId, context) 
    {
        _IdEntityProperty = new IdProperty(this, s_Id, typeof(Int32));
        _NameEntityProperty = new NameProperty(this, s_Name, typeof(String));
        _SauceEntityProperty = new SauceProperty(this, s_Sauce, typeof(Sauce));
        _ToppingsEntityProperty = new ToppingsProperty(this, s_Toppings, typeof(ICollection<Topping>));
        _entity = new PizzaPocotaEntity(this);
    }
    protected override IEnumerable<EntityProperty> GetProperties()
    {
        yield return _IdEntityProperty;
        yield return _NameEntityProperty;
        yield return _SauceEntityProperty;
        yield return _ToppingsEntityProperty;
    }
}
