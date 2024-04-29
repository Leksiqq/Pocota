/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.Sauce                        //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-29T15:06:27.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Client;
using System;

namespace ContosoPizza.Models.Client;


public class Sauce: PocotaEntity, ISaucePocotaEntity
{
    private readonly EntityProperty _IdEntityProperty = new();
    private readonly EntityProperty _Id1EntityProperty = new();
    private readonly EntityProperty _NameEntityProperty = new();
    private readonly EntityProperty _IsVeganEntityProperty = new();
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
