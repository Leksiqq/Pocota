/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Topping                      //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-30T18:11:42.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Net.Leksi.Pocota.Client;
using Net.Leksi.Pocota.Contract;
using System;
using System.Collections.Generic;

namespace ContosoPizza.Models.Client;


public class Topping: IEntityOwner
{
    private class IdProperty(IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get 
            {
                if(Access is AccessKind.NotSet || Access is AccessKind.Forbidden)
                {
                    return default;
                }
                return _value ?? default;
            }
            set 
            {
                if (!IsReadonly)
                {
                    if(value is Int32 val && val != ((Topping)Entity).Id)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                    }
                    else if(value == default && _value != default)
                    {
                        _value = default;
                        NotifyPropertyChanged();
                    }
                }
            }
        }
        public override bool IsNullable => false;
    }
    private class NameProperty(IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get 
            {
                if(Access is AccessKind.NotSet || Access is AccessKind.Forbidden)
                {
                    return default;
                }
                return _value ?? default;
            }
            set 
            {
                if (!IsReadonly)
                {
                    if(value is String val && val != ((Topping)Entity).Name)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                    }
                    else if(value == default && _value != default)
                    {
                        _value = default;
                        NotifyPropertyChanged();
                    }
                }
            }
        }
        public override bool IsNullable => true;
    }
    private class CaloriesProperty(IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get 
            {
                if(Access is AccessKind.NotSet || Access is AccessKind.Forbidden)
                {
                    return default;
                }
                return _value ?? default;
            }
            set 
            {
                if (!IsReadonly)
                {
                    if(value is Decimal val && val != ((Topping)Entity).Calories)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                    }
                    else if(value == default && _value != default)
                    {
                        _value = default;
                        NotifyPropertyChanged();
                    }
                }
            }
        }
        public override bool IsNullable => true;
    }
    private class PizzasProperty(IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get 
            {
                if(Access is AccessKind.NotSet || Access is AccessKind.Forbidden)
                {
                    return default;
                }
                return _value ?? default;
            }
            set 
            {
                if (!IsReadonly)
                {
                    if(value is IList<Pizza> val && val != ((Topping)Entity).Pizzas)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                    }
                    else if(value == default && _value != default)
                    {
                        _value = default;
                        NotifyPropertyChanged();
                    }
                }
            }
        }
        public override bool IsNullable => true;
    }
    private class ToppingPocotaEntity: PocotaEntity, IToppingPocotaEntity
    {
        public EntityProperty Id { get; private init;}
        public EntityProperty Name { get; private init;}
        public EntityProperty Calories { get; private init;}
        public EntityProperty Pizzas { get; private init;}
        internal ToppingPocotaEntity(ulong pocotaId, PocotaContext context): base(pocotaId, context) 
        {
            Id = new IdProperty(this, s_Id, typeof(Int32));
            Name = new NameProperty(this, s_Name, typeof(String));
            Calories = new CaloriesProperty(this, s_Calories, typeof(Decimal));
            Pizzas = new PizzasProperty(this, s_Pizzas, typeof(IList<Pizza>));
        }
        protected override IEnumerable<EntityProperty> GetProperties()
        {
            yield return Id;
            yield return Name;
            yield return Calories;
            yield return Pizzas;
        }
    }
    private const string s_Id = "Id";
    private const string s_Name = "Name";
    private const string s_Calories = "Calories";
    private const string s_Pizzas = "Pizzas";
    private readonly ToppingPocotaEntity _entity;
    public Int32 Id 
    { 
        get => (Int32)_entity.Id.Value!; 
        set => _entity.Id.Value = value; 
    }
    public String? Name 
    { 
        get => (String?)_entity.Name.Value; 
        set => _entity.Name.Value = value; 
    }
    public Decimal? Calories 
    { 
        get => (Decimal?)_entity.Calories.Value; 
        set => _entity.Calories.Value = value; 
    }
    public IList<Pizza>? Pizzas 
    { 
        get => (IList<Pizza>?)_entity.Pizzas.Value; 
        set => _entity.Pizzas.Value = value; 
    }
    IPocotaEntity IEntityOwner.Entity => _entity;
    internal Topping(ulong pocotaId, PocotaContext context)
    {
        _entity = new ToppingPocotaEntity(pocotaId, context);
    }
}
