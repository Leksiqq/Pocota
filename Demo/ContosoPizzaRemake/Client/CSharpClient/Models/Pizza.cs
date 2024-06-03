/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Pizza                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-03T16:59:17.                                 //
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


public class Pizza: IEntityOwner, INotifyPropertyChanged
{
    private class IdProperty(Pizza owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
    private class NameProperty(Pizza owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
    private class SauceProperty(Pizza owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
                    if(value is Sauce val && val != owner.Sauce)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _SaucePropertyChangedEventArgs);
                    }
                    else if((Sauce)value! == default && (Sauce)_value! != default)
                    {
                        _value = default(Sauce);
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _SaucePropertyChangedEventArgs);
                    }
                }
            }
        }
        public override bool IsNullable => true;
    }
    private class ToppingsProperty(Pizza owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
                    if(value is ObservableCollection<Topping> val && val != owner.Toppings)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _ToppingsPropertyChangedEventArgs);
                    }
                    else if((ObservableCollection<Topping>)value! == default && (ObservableCollection<Topping>)_value! != default)
                    {
                        _value = default(ObservableCollection<Topping>);
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _ToppingsPropertyChangedEventArgs);
                    }
                }
            }
        }
        public override bool IsNullable => true;
    }
    private class PizzaPocotaEntity: PocotaEntity, IPizzaPocotaEntity
    {
        public EntityProperty Id { get; private init;}
        public EntityProperty Name { get; private init;}
        public EntityProperty Sauce { get; private init;}
        public EntityProperty Toppings { get; private init;}
        internal PizzaPocotaEntity(ulong pocotaId, PocotaContext context, Pizza owner): base(pocotaId, context) 
        {
            Id = new IdProperty(owner, this, s_Id, typeof(Int32));
            Name = new NameProperty(owner, this, s_Name, typeof(String));
            Sauce = new SauceProperty(owner, this, s_Sauce, typeof(Sauce));
            Toppings = new ToppingsProperty(owner, this, s_Toppings, typeof(ObservableCollection<Topping>));
        }
        protected override IEnumerable<EntityProperty> GetProperties()
        {
            yield return Id;
            yield return Name;
            yield return Sauce;
            yield return Toppings;
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    private const string s_Id = "Id";
    private static readonly PropertyChangedEventArgs _IdPropertyChangedEventArgs = new(s_Id);
    private const string s_Name = "Name";
    private static readonly PropertyChangedEventArgs _NamePropertyChangedEventArgs = new(s_Name);
    private const string s_Sauce = "Sauce";
    private static readonly PropertyChangedEventArgs _SaucePropertyChangedEventArgs = new(s_Sauce);
    private const string s_Toppings = "Toppings";
    private static readonly PropertyChangedEventArgs _ToppingsPropertyChangedEventArgs = new(s_Toppings);
    private readonly PizzaPocotaEntity _entity;
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
    public Sauce? Sauce 
    { 
        get => (Sauce?)_entity.Sauce.Value; 
        set => _entity.Sauce.Value = value; 
    }
    public ObservableCollection<Topping>? Toppings 
    { 
        get => (ObservableCollection<Topping>?)_entity.Toppings.Value; 
        set => _entity.Toppings.Value = value; 
    }
    IPocotaEntity IEntityOwner.Entity => _entity;
    internal Pizza(ulong pocotaId, PocotaContext context)
    {
        _entity = new PizzaPocotaEntity(pocotaId, context, this);
    }
}
