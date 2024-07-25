/////////////////////////////////////////////////////////////
// ContosoPizza.Client.GetPizzaOptions                     //
// was generated automatically from ContosoPizza.IContract //
// at 2024-07-25T16:23:24.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using ContosoPizza.Models.Client;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Client;


public class GetPizzaOptions: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _PizzaPropertyChangedEventArgs = new(nameof(Pizza));
    private Pizza _Pizza = default!;
    public Pizza Pizza 
    { 
        get => _Pizza; 
        set
        {
            if(_Pizza != value)
            {
                _Pizza = value;
                PropertyChanged?.Invoke(this, _PizzaPropertyChangedEventArgs);
            }
        }
    }
}