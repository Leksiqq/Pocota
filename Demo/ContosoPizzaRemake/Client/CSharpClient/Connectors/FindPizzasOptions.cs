/////////////////////////////////////////////////////////////
// ContosoPizza.Client.FindPizzasOptions                   //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-20T18:20:03.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Client;


public class FindPizzasOptions: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _FilterPropertyChangedEventArgs = new(nameof(Filter));
    private static readonly PropertyChangedEventArgs _StagePropertyChangedEventArgs = new(nameof(Stage));
    private static readonly PropertyChangedEventArgs _SurePropertyChangedEventArgs = new(nameof(Sure));
    private PizzaFilter _Filter = default!;
    private Int32 _Stage = default!;
    private Boolean? _Sure = default;
    public PizzaFilter Filter 
    { 
        get => _Filter; 
        set
        {
            if(_Filter != value)
            {
                _Filter = value;
                PropertyChanged?.Invoke(this, _FilterPropertyChangedEventArgs);
            }
        }
    }
    public Int32 Stage 
    { 
        get => _Stage; 
        set
        {
            if(_Stage != value)
            {
                _Stage = value;
                PropertyChanged?.Invoke(this, _StagePropertyChangedEventArgs);
            }
        }
    }
    public Boolean? Sure 
    { 
        get => _Sure; 
        set
        {
            if(_Sure != value)
            {
                _Sure = value;
                PropertyChanged?.Invoke(this, _SurePropertyChangedEventArgs);
            }
        }
    }
}