/////////////////////////////////////////////////////////////
// ContosoPizza.Client.GetAllSaucesOptions                 //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-21T16:41:59.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Client;


public class GetAllSaucesOptions: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _LimitPropertyChangedEventArgs = new(nameof(Limit));
    private Int32 _Limit = default!;
    public Int32 Limit 
    { 
        get => _Limit; 
        set
        {
            if(_Limit != value)
            {
                _Limit = value;
                PropertyChanged?.Invoke(this, _LimitPropertyChangedEventArgs);
            }
        }
    }
}