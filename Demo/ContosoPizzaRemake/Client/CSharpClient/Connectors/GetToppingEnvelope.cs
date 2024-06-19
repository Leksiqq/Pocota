/////////////////////////////////////////////////////////////
// ContosoPizza.Client.GetToppingEnvelope                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-19T16:46:35.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using ContosoPizza.Models.Client;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Client;


public class GetToppingEnvelope: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _toppingPropertyChangedEventArgs = new(nameof(topping));
    private Topping _topping = default!;
    public Topping topping 
    { 
        get => _topping; 
        set
        {
            if(_topping != value)
            {
                _topping = value;
                PropertyChanged?.Invoke(this, _toppingPropertyChangedEventArgs);
            }
        }
    }
}