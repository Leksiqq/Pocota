/////////////////////////////////////////////////////////////
// ContosoPizza.Client.GetToppingOptions                   //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-21T16:41:59.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using ContosoPizza.Models.Client;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Client;


public class GetToppingOptions: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _ToppingPropertyChangedEventArgs = new(nameof(Topping));
    private Topping _Topping = default!;
    public Topping Topping 
    { 
        get => _Topping; 
        set
        {
            if(_Topping != value)
            {
                _Topping = value;
                PropertyChanged?.Invoke(this, _ToppingPropertyChangedEventArgs);
            }
        }
    }
}