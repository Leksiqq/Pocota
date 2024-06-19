/////////////////////////////////////////////////////////////
// ContosoPizza.Client.GetSauceEnvelope                    //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-19T17:22:13.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using ContosoPizza.Models.Client;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Client;


public class GetSauceEnvelope: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _saucePropertyChangedEventArgs = new(nameof(sauce));
    private Sauce _sauce = default!;
    public Sauce sauce 
    { 
        get => _sauce; 
        set
        {
            if(_sauce != value)
            {
                _sauce = value;
                PropertyChanged?.Invoke(this, _saucePropertyChangedEventArgs);
            }
        }
    }
}