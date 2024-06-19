/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Sauce                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-19T16:46:35.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Client;
using Net.Leksi.Pocota.Contract;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Models.Client;


public class Sauce: IEntityOwner, INotifyPropertyChanged
{
    private class SaucePocotaEntity: PocotaEntity, ISaucePocotaEntity
    {
        public EntityProperty Id { get; private init;}
        public EntityProperty Id1 { get; private init;}
        public EntityProperty Name { get; private init;}
        public EntityProperty IsVegan { get; private init;}
        internal SaucePocotaEntity(ulong pocotaId, PocotaContext context, Sauce owner): base(pocotaId, context, owner) 
        {
            Id = new EntityProperty(this, s_Id, typeof(Int32));
            Id1 = new EntityProperty(this, s_Id1, typeof(Int32));
            Name = new EntityProperty(this, s_Name, typeof(String));
            IsVegan = new EntityProperty(this, s_IsVegan, typeof(Boolean));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    private const string s_Id = nameof(Id);
    private const string s_Id1 = nameof(Id1);
    private const string s_Name = nameof(Name);
    private const string s_IsVegan = nameof(IsVegan);
    private static readonly PropertyChangedEventArgs _IdPropertyChangedEventArgs = new(s_Id);
    private static readonly PropertyChangedEventArgs _Id1PropertyChangedEventArgs = new(s_Id1);
    private static readonly PropertyChangedEventArgs _NamePropertyChangedEventArgs = new(s_Name);
    private static readonly PropertyChangedEventArgs _IsVeganPropertyChangedEventArgs = new(s_IsVegan);
    private readonly SaucePocotaEntity _entity;
    private Int32 _Id = default!;
    private Int32 _Id1 = default!;
    private String? _Name = default;
    private Boolean _IsVegan = default!;
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
    public Int32 Id1 
    { 
        get => _Id1; 
        set
        {
            if(_Id1 != value && !_entity.Id1.IsReadonly)
            {
                _Id1 = value;
                PropertyChanged?.Invoke(this, _Id1PropertyChangedEventArgs);
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
    public Boolean IsVegan 
    { 
        get => _IsVegan; 
        set
        {
            if(_IsVegan != value && !_entity.IsVegan.IsReadonly)
            {
                _IsVegan = value;
                PropertyChanged?.Invoke(this, _IsVeganPropertyChangedEventArgs);
            }
        }
    }
    IPocotaEntity IEntityOwner.Entity => _entity;
    internal Sauce(ulong pocotaId, PocotaContext context)
    {
        _entity = new SaucePocotaEntity(pocotaId, context, this);
    }
}
