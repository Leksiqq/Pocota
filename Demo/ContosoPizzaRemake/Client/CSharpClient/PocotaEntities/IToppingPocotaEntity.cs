/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.IToppingPocotaEntity         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-15T08:59:48.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Client;
using System;

namespace ContosoPizza.Models.Client;


public interface IToppingPocotaEntity: IPocotaEntity
{
    EntityProperty Id { get; }
    EntityProperty Name { get; }
    EntityProperty Calories { get; }
    EntityProperty Pizzas { get; }
}