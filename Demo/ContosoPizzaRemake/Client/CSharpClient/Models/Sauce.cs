/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Sauce                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-08T20:36:28.                                 //
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
    private readonly EntityProperty _IdEntityProperty = new(typeof(Sauce), s_Id);
    private readonly EntityProperty _Id1EntityProperty = new(typeof(Sauce), s_Id1);
    private readonly EntityProperty _NameEntityProperty = new(typeof(Sauce), s_Name);
    private readonly EntityProperty _IsVeganEntityProperty = new(typeof(Sauce), s_IsVegan);
    public Int32 Id { get; set; }
    public Int32 Id1 { get; set; }
    public String? Name { get; set; }
    public Boolean IsVegan { get; set; }
    EntityProperty ISaucePocotaEntity.Id => _IdEntityProperty;
    EntityProperty ISaucePocotaEntity.Id1 => _Id1EntityProperty;
    EntityProperty ISaucePocotaEntity.Name => _NameEntityProperty;
    EntityProperty ISaucePocotaEntity.IsVegan => _IsVeganEntityProperty;
    internal Sauce(ulong pocotaId): base(pocotaId) { }
}
