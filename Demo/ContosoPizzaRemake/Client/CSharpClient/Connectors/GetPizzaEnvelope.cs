/////////////////////////////////////////////////////////////
// ContosoPizza.Client.GetPizzaEnvelope                    //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-19T17:22:13.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using ContosoPizza.Models.Client;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Client;


public class GetPizzaEnvelope: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _pizzaPropertyChangedEventArgs = new(nameof(pizza));
    private Pizza _pizza = default!;
    public Pizza pizza 
    { 
        get => _pizza; 
        set
        {
            if(_pizza != value)
            {
                _pizza = value;
                PropertyChanged?.Invoke(this, _pizzaPropertyChangedEventArgs);
            }
        }
    }
}