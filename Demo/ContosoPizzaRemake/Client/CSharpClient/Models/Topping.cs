/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Topping                      //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-14T17:14:56.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Net.Leksi.Pocota.Client;
using Net.Leksi.Pocota.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ContosoPizza.Models.Client;


public class Topping: IEntityOwner, INotifyPropertyChanged
{
    private class IdProperty(Topping owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get 
            {
                if(Access is AccessKind.NotSet || Access is AccessKind.Forbidden)
                {
                    return default;
                }
                return _value;
            }
            set 
            {
                if (!IsReadonly)
                {
                    if(value is Int32 val && val != owner.Id)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _IdPropertyChangedEventArgs);
                    }
                    else if((Int32)value! == default && (Int32)_value! != default)
                    {
                        _value = default(Int32);
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _IdPropertyChangedEventArgs);
                    }
                }
            }
        }
        public override bool IsNullable => false;
    }
    private class NameProperty(Topping owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get 
            {
                if(Access is AccessKind.NotSet || Access is AccessKind.Forbidden)
                {
                    return default;
                }
                return _value;
            }
            set 
            {
                if (!IsReadonly)
                {
                    if(value is String val && val != owner.Name)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _NamePropertyChangedEventArgs);
                    }
                    else if((String)value! == default && (String)_value! != default)
                    {
                        _value = default(String);
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _NamePropertyChangedEventArgs);
                    }
                }
            }
        }
        public override bool IsNullable => true;
    }
    private class CaloriesProperty(Topping owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get 
            {
                if(Access is AccessKind.NotSet || Access is AccessKind.Forbidden)
                {
                    return default;
                }
                return _value;
            }
            set 
            {
                if (!IsReadonly)
                {
                    if(value is Decimal val && val != owner.Calories)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _CaloriesPropertyChangedEventArgs);
                    }
                    else if((Decimal)value! == default && (Decimal)_value! != default)
                    {
                        _value = default(Decimal);
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _CaloriesPropertyChangedEventArgs);
                    }
                }
            }
        }
        public override bool IsNullable => true;
    }
    private class PizzasProperty(Topping owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get 
            {
                if(Access is AccessKind.NotSet || Access is AccessKind.Forbidden)
                {
                    return default;
                }
                return _value;
            }
            set 
            {
                if (!IsReadonly)
                {
                    if(value is ObservableCollection<Pizza> val && val != owner.Pizzas)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _PizzasPropertyChangedEventArgs);
                    }
                    else if((ObservableCollection<Pizza>)value! == default && (ObservableCollection<Pizza>)_value! != default)
                    {
                        _value = default(ObservableCollection<Pizza>);
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _PizzasPropertyChangedEventArgs);
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
        internal ToppingPocotaEntity(ulong pocotaId, PocotaContext context, Topping owner): base(pocotaId, context) 
        {
            Id = new IdProperty(owner, this, s_Id, typeof(Int32));
            Name = new NameProperty(owner, this, s_Name, typeof(String));
            Calories = new CaloriesProperty(owner, this, s_Calories, typeof(Decimal));
            Pizzas = new PizzasProperty(owner, this, s_Pizzas, typeof(ObservableCollection<Pizza>));
        }
        protected override IEnumerable<EntityProperty> GetProperties()
        {
            yield return Id;
            yield return Name;
            yield return Calories;
            yield return Pizzas;
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    private const string s_Id = "Id";
    private static readonly PropertyChangedEventArgs _IdPropertyChangedEventArgs = new(s_Id);
    private const string s_Name = "Name";
    private static readonly PropertyChangedEventArgs _NamePropertyChangedEventArgs = new(s_Name);
    private const string s_Calories = "Calories";
    private static readonly PropertyChangedEventArgs _CaloriesPropertyChangedEventArgs = new(s_Calories);
    private const string s_Pizzas = "Pizzas";
    private static readonly PropertyChangedEventArgs _PizzasPropertyChangedEventArgs = new(s_Pizzas);
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
    public ObservableCollection<Pizza>? Pizzas 
    { 
        get => (ObservableCollection<Pizza>?)_entity.Pizzas.Value; 
        set => _entity.Pizzas.Value = value; 
    }
    IPocotaEntity IEntityOwner.Entity => _entity;
    internal Topping(ulong pocotaId, PocotaContext context)
    {
        _entity = new ToppingPocotaEntity(pocotaId, context, this);
    }
}
