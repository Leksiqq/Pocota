/////////////////////////////////////////////////////////////
// ContosoPizza.Client.GetSauceOptions                     //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-21T16:41:59.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using ContosoPizza.Models.Client;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Client;


public class GetSauceOptions: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _SaucePropertyChangedEventArgs = new(nameof(Sauce));
    private Sauce _Sauce = default!;
    public Sauce Sauce 
    { 
        get => _Sauce; 
        set
        {
            if(_Sauce != value)
            {
                _Sauce = value;
                PropertyChanged?.Invoke(this, _SaucePropertyChangedEventArgs);
            }
        }
    }
}