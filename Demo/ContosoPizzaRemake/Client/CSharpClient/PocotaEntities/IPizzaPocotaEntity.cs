/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.IPizzaPocotaEntity           //
// was generated automatically from ContosoPizza.IContract //
// at 2024-05-15T10:42:54.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Client;
using System;

namespace ContosoPizza.Models.Client;


public interface IPizzaPocotaEntity: IPocotaEntity
{
    EntityProperty Id { get; }
    EntityProperty Name { get; }
    EntityProperty Sauce { get; }
    EntityProperty Toppings { get; }
}