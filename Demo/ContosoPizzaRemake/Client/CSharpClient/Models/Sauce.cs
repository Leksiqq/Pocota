/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Sauce                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-20T15:57:53.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Client;
using System;

namespace ContosoPizza.Models.Client;


public class Sauce: PocotaEntity, ISaucePocotaEntity
{
    private class IdProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Sauce)Entity)?.Id;
            set {
                if(Entity is {} && value is Int32 val && val != ((Sauce)Entity).Id) 
                {
                    ((Sauce)Entity).Id = val;
                    NotifyPropertyChanged();
                }
            }
        }
    }
    private class Id1Property(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Sauce)Entity)?.Id1;
            set {
                if(Entity is {} && value is Int32 val && val != ((Sauce)Entity).Id1) 
                {
                    ((Sauce)Entity).Id1 = val;
                    NotifyPropertyChanged();
                }
            }
        }
    }
    private class NameProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Sauce)Entity)?.Name;
            set {
                if(Entity is {} && value is String val && val != ((Sauce)Entity).Name) 
                {
                    ((Sauce)Entity).Name = val;
                    NotifyPropertyChanged();
                }
            }
        }
    }
    private class IsVeganProperty(PocotaEntity entity, string name, Type type): EntityProperty(entity, name, type)
    {
        public override object? Value 
        {
            get => ((Sauce)Entity)?.IsVegan;
            set {
                if(Entity is {} && value is Boolean val && val != ((Sauce)Entity).IsVegan) 
                {
                    ((Sauce)Entity).IsVegan = val;
                    NotifyPropertyChanged();
                }
            }
        }
    }
    private const string s_Id = "Id";
    private const string s_Id1 = "Id1";
    private const string s_Name = "Name";
    private const string s_IsVegan = "IsVegan";
    private readonly IdProperty _IdEntityProperty;
    private readonly Id1Property _Id1EntityProperty;
    private readonly NameProperty _NameEntityProperty;
    private readonly IsVeganProperty _IsVeganEntityProperty;
    public Int32 Id { get; set; }
    public Int32 Id1 { get; set; }
    public String? Name { get; set; }
    public Boolean IsVegan { get; set; }
    EntityProperty ISaucePocotaEntity.Id => _IdEntityProperty;
    EntityProperty ISaucePocotaEntity.Id1 => _Id1EntityProperty;
    EntityProperty ISaucePocotaEntity.Name => _NameEntityProperty;
    EntityProperty ISaucePocotaEntity.IsVegan => _IsVeganEntityProperty;
    internal Sauce(ulong pocotaId, PocotaContext context): base(pocotaId, context) 
    {
        _IdEntityProperty = new IdProperty(this, s_Id, typeof(Int32));
        _Id1EntityProperty = new Id1Property(this, s_Id1, typeof(Int32));
        _NameEntityProperty = new NameProperty(this, s_Name, typeof(String));
        _IsVeganEntityProperty = new IsVeganProperty(this, s_IsVegan, typeof(Boolean));
    }
    protected override IEnumerable<EntityProperty> GetProperties()
    {
        yield return _IdEntityProperty;
        yield return _Id1EntityProperty;
        yield return _NameEntityProperty;
        yield return _IsVeganEntityProperty;
    }
}
