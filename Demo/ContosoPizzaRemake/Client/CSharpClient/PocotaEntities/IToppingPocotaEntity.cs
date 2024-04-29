/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.IToppingPocotaEntity         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-29T15:06:28.                                 //
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