/////////////////////////////////////////////////////////////
// ContosoPizza.Models.Client.IToppingPocotaEntity         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-10T13:02:54.                                 //
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