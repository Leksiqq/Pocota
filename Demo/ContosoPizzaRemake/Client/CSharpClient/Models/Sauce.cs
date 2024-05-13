/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Sauce                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-13T17:59:08.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Client;
using System;

namespace ContosoPizza.Models.Client;


public class Sauce: PocotaEntity, ISaucePocotaEntity
{
    private const string s_Id = "Id";
    private const string s_Id1 = "Id1";
    private const string s_Name = "Name";
    private const string s_IsVegan = "IsVegan";
    private readonly EntityProperty _IdEntityProperty;
    private readonly EntityProperty _Id1EntityProperty;
    private readonly EntityProperty _NameEntityProperty;
    private readonly EntityProperty _IsVeganEntityProperty;
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
        _IdEntityProperty = new EntityProperty(this, s_Id);
        _Id1EntityProperty = new EntityProperty(this, s_Id1);
        _NameEntityProperty = new EntityProperty(this, s_Name);
        _IsVeganEntityProperty = new EntityProperty(this, s_IsVegan);
    }
}
