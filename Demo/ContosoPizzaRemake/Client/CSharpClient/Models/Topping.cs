/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Topping                      //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-19T16:46:35.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using Net.Leksi.Pocota.Client;
using Net.Leksi.Pocota.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Models.Client;


public class Topping: IEntityOwner, INotifyPropertyChanged
{
    private class ToppingPocotaEntity: PocotaEntity, IToppingPocotaEntity
    {
        public EntityProperty Id { get; private init;}
        public EntityProperty Name { get; private init;}
        public EntityProperty Calories { get; private init;}
        public EntityProperty Pizzas { get; private init;}
        internal ToppingPocotaEntity(ulong pocotaId, PocotaContext context, Topping owner): base(pocotaId, context, owner) 
        {
            Id = new EntityProperty(this, s_Id, typeof(Int32));
            Name = new EntityProperty(this, s_Name, typeof(String));
            Calories = new EntityProperty(this, s_Calories, typeof(Decimal));
            Pizzas = new EntityProperty(this, s_Pizzas, typeof(ObservableCollection<Pizza>));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    private const string s_Id = nameof(Id);
    private const string s_Name = nameof(Name);
    private const string s_Calories = nameof(Calories);
    private const string s_Pizzas = nameof(Pizzas);
    private static readonly PropertyChangedEventArgs _IdPropertyChangedEventArgs = new(s_Id);
    private static readonly PropertyChangedEventArgs _NamePropertyChangedEventArgs = new(s_Name);
    private static readonly PropertyChangedEventArgs _CaloriesPropertyChangedEventArgs = new(s_Calories);
    private static readonly PropertyChangedEventArgs _PizzasPropertyChangedEventArgs = new(s_Pizzas);
    private readonly ToppingPocotaEntity _entity;
    private Int32 _Id = default!;
    private String? _Name = default;
    private Decimal? _Calories = default;
    private readonly ObservableCollection<Pizza> _Pizzas;
    public Int32 Id 
    { 
        get => _Id; 
        set
        {
            if(_Id != value && !_entity.Id.IsReadonly)
            {
                _Id = value;
                PropertyChanged?.Invoke(this, _IdPropertyChangedEventArgs);
            }
        }
    }
    public String? Name 
    { 
        get => _Name; 
        set
        {
            if(_Name != value && !_entity.Name.IsReadonly)
            {
                _Name = value;
                PropertyChanged?.Invoke(this, _NamePropertyChangedEventArgs);
            }
        }
    }
    public Decimal? Calories 
    { 
        get => _Calories; 
        set
        {
            if(_Calories != value && !_entity.Calories.IsReadonly)
            {
                _Calories = value;
                PropertyChanged?.Invoke(this, _CaloriesPropertyChangedEventArgs);
            }
        }
    }
    public ObservableCollection<Pizza> Pizzas 
    { 
        get => _Pizzas; 
    }
    IPocotaEntity IEntityOwner.Entity => _entity;
    internal Topping(ulong pocotaId, PocotaContext context)
    {
        _entity = new ToppingPocotaEntity(pocotaId, context, this);
       _Pizzas = new MyObservableCollection<Pizza>(_entity.Pizzas); 
    }
}
