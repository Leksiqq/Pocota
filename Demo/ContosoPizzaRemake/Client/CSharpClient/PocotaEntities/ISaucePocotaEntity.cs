/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.ISaucePocotaEntity           //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-15T10:42:54.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Client;
using System;

namespace ContosoPizza.Models.Client;


public interface ISaucePocotaEntity: IPocotaEntity
{
    EntityProperty Id { get; }
    EntityProperty Id1 { get; }
    EntityProperty Name { get; }
    EntityProperty IsVegan { get; }
}