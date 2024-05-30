/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Sauce                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-30T18:11:42.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Client;
using Net.Leksi.Pocota.Contract;
using System;

namespace ContosoPizza.Models.Client;


public class Sauce: IEntityOwner
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
                    if(value is Int32 val && val != ((Sauce)Entity).Id)
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
    private class Id1Property(IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
                    if(value is Int32 val && val != ((Sauce)Entity).Id1)
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
                    if(value is String val && val != ((Sauce)Entity).Name)
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
    private class IsVeganProperty(IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
                    if(value is Boolean val && val != ((Sauce)Entity).IsVegan)
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
    private class SaucePocotaEntity: PocotaEntity, ISaucePocotaEntity
    {
        public EntityProperty Id { get; private init;}
        public EntityProperty Id1 { get; private init;}
        public EntityProperty Name { get; private init;}
        public EntityProperty IsVegan { get; private init;}
        internal SaucePocotaEntity(ulong pocotaId, PocotaContext context): base(pocotaId, context) 
        {
            Id = new IdProperty(this, s_Id, typeof(Int32));
            Id1 = new Id1Property(this, s_Id1, typeof(Int32));
            Name = new NameProperty(this, s_Name, typeof(String));
            IsVegan = new IsVeganProperty(this, s_IsVegan, typeof(Boolean));
        }
        protected override IEnumerable<EntityProperty> GetProperties()
        {
            yield return Id;
            yield return Id1;
            yield return Name;
            yield return IsVegan;
        }
    }
    private const string s_Id = "Id";
    private const string s_Id1 = "Id1";
    private const string s_Name = "Name";
    private const string s_IsVegan = "IsVegan";
    private readonly SaucePocotaEntity _entity;
    public Int32 Id 
    { 
        get => (Int32)_entity.Id.Value!; 
        set => _entity.Id.Value = value; 
    }
    public Int32 Id1 
    { 
        get => (Int32)_entity.Id1.Value!; 
        set => _entity.Id1.Value = value; 
    }
    public String? Name 
    { 
        get => (String?)_entity.Name.Value; 
        set => _entity.Name.Value = value; 
    }
    public Boolean IsVegan 
    { 
        get => (Boolean)_entity.IsVegan.Value!; 
        set => _entity.IsVegan.Value = value; 
    }
    IPocotaEntity IEntityOwner.Entity => _entity;
    internal Sauce(ulong pocotaId, PocotaContext context)
    {
        _entity = new SaucePocotaEntity(pocotaId, context);
    }
}
