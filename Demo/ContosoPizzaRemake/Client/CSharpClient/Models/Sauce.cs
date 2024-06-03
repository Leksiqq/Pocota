/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Sauce                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-03T16:59:17.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Client;
using Net.Leksi.Pocota.Contract;
using System;
using System.ComponentModel;

namespace ContosoPizza.Models.Client;


public class Sauce: IEntityOwner, INotifyPropertyChanged
{
    private class IdProperty(Sauce owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
    private class Id1Property(Sauce owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
                    if(value is Int32 val && val != owner.Id1)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _Id1PropertyChangedEventArgs);
                    }
                    else if((Int32)value! == default && (Int32)_value! != default)
                    {
                        _value = default(Int32);
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _Id1PropertyChangedEventArgs);
                    }
                }
            }
        }
        public override bool IsNullable => false;
    }
    private class NameProperty(Sauce owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
    private class IsVeganProperty(Sauce owner, IPocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
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
                    if(value is Boolean val && val != owner.IsVegan)
                    {
                        _value = val;
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _IsVeganPropertyChangedEventArgs);
                    }
                    else if((Boolean)value! == default && (Boolean)_value! != default)
                    {
                        _value = default(Boolean);
                        NotifyPropertyChanged();
                        owner.PropertyChanged?.Invoke(owner, _IsVeganPropertyChangedEventArgs);
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
        internal SaucePocotaEntity(ulong pocotaId, PocotaContext context, Sauce owner): base(pocotaId, context) 
        {
            Id = new IdProperty(owner, this, s_Id, typeof(Int32));
            Id1 = new Id1Property(owner, this, s_Id1, typeof(Int32));
            Name = new NameProperty(owner, this, s_Name, typeof(String));
            IsVegan = new IsVeganProperty(owner, this, s_IsVegan, typeof(Boolean));
        }
        protected override IEnumerable<EntityProperty> GetProperties()
        {
            yield return Id;
            yield return Id1;
            yield return Name;
            yield return IsVegan;
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    private const string s_Id = "Id";
    private static readonly PropertyChangedEventArgs _IdPropertyChangedEventArgs = new(s_Id);
    private const string s_Id1 = "Id1";
    private static readonly PropertyChangedEventArgs _Id1PropertyChangedEventArgs = new(s_Id1);
    private const string s_Name = "Name";
    private static readonly PropertyChangedEventArgs _NamePropertyChangedEventArgs = new(s_Name);
    private const string s_IsVegan = "IsVegan";
    private static readonly PropertyChangedEventArgs _IsVeganPropertyChangedEventArgs = new(s_IsVegan);
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
        _entity = new SaucePocotaEntity(pocotaId, context, this);
    }
}
