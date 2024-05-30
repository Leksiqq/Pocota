/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Pizza                        //
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


public class Pizza: IEntityOwner
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
                    if(value is Int32 val && val != ((Pizza)Entity).Id)
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
                    if(value is String val && val != ((Pizza)Entity).Name)
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
    private class SauceProperty(IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
                    if(value is Sauce val && val != ((Pizza)Entity).Sauce)
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
    private class ToppingsProperty(IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
                    if(value is IList<Topping> val && val != ((Pizza)Entity).Toppings)
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
    private class PizzaPocotaEntity: PocotaEntity, IPizzaPocotaEntity
    {
        public EntityProperty Id { get; private init;}
        public EntityProperty Name { get; private init;}
        public EntityProperty Sauce { get; private init;}
        public EntityProperty Toppings { get; private init;}
        internal PizzaPocotaEntity(ulong pocotaId, PocotaContext context): base(pocotaId, context) 
        {
            Id = new IdProperty(this, s_Id, typeof(Int32));
            Name = new NameProperty(this, s_Name, typeof(String));
            Sauce = new SauceProperty(this, s_Sauce, typeof(Sauce));
            Toppings = new ToppingsProperty(this, s_Toppings, typeof(IList<Topping>));
        }
        protected override IEnumerable<EntityProperty> GetProperties()
        {
            yield return Id;
            yield return Name;
            yield return Sauce;
            yield return Toppings;
        }
    }
    private const string s_Id = "Id";
    private const string s_Name = "Name";
    private const string s_Sauce = "Sauce";
    private const string s_Toppings = "Toppings";
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
    public IList<Topping>? Toppings 
    { 
        get => (IList<Topping>?)_entity.Toppings.Value; 
        set => _entity.Toppings.Value = value; 
    }
    IPocotaEntity IEntityOwner.Entity => _entity;
    internal Pizza(ulong pocotaId, PocotaContext context)
    {
        _entity = new PizzaPocotaEntity(pocotaId, context);
    }
}
