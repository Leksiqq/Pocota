/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Pizza                        //
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


public class Pizza: IEntityOwner, INotifyPropertyChanged
{
    private class PizzaPocotaEntity: PocotaEntity, IPizzaPocotaEntity
    {
        public EntityProperty Id { get; private init;}
        public EntityProperty Name { get; private init;}
        public EntityProperty Sauce { get; private init;}
        public EntityProperty Toppings { get; private init;}
        internal PizzaPocotaEntity(ulong pocotaId, PocotaContext context, Pizza owner): base(pocotaId, context, owner) 
        {
            Id = new EntityProperty(this, s_Id, typeof(Int32));
            Name = new EntityProperty(this, s_Name, typeof(String));
            Sauce = new EntityProperty(this, s_Sauce, typeof(Sauce));
            Toppings = new EntityProperty(this, s_Toppings, typeof(ObservableCollection<Topping>));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    private const string s_Id = nameof(Id);
    private const string s_Name = nameof(Name);
    private const string s_Sauce = nameof(Sauce);
    private const string s_Toppings = nameof(Toppings);
    private static readonly PropertyChangedEventArgs _IdPropertyChangedEventArgs = new(s_Id);
    private static readonly PropertyChangedEventArgs _NamePropertyChangedEventArgs = new(s_Name);
    private static readonly PropertyChangedEventArgs _SaucePropertyChangedEventArgs = new(s_Sauce);
    private static readonly PropertyChangedEventArgs _ToppingsPropertyChangedEventArgs = new(s_Toppings);
    private readonly PizzaPocotaEntity _entity;
    private Int32 _Id = default!;
    private String? _Name = default;
    private Sauce? _Sauce = default;
    private readonly ObservableCollection<Topping> _Toppings;
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
    public Sauce? Sauce 
    { 
        get => _Sauce; 
        set
        {
            if(_Sauce != value && !_entity.Sauce.IsReadonly)
            {
                _Sauce = value;
                PropertyChanged?.Invoke(this, _SaucePropertyChangedEventArgs);
            }
        }
    }
    public ObservableCollection<Topping> Toppings 
    { 
        get => _Toppings; 
    }
    IPocotaEntity IEntityOwner.Entity => _entity;
    internal Pizza(ulong pocotaId, PocotaContext context)
    {
        _entity = new PizzaPocotaEntity(pocotaId, context, this);
       _Toppings = new MyObservableCollection<Topping>(_entity.Toppings); 
    }
}
