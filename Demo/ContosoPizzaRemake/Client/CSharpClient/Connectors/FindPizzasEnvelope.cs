/////////////////////////////////////////////////////////////
// ContosoPizza.Client.FindPizzasEnvelope                  //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-19T17:22:13.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Client;


public class FindPizzasEnvelope: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _filterPropertyChangedEventArgs = new(nameof(filter));
    private static readonly PropertyChangedEventArgs _stagePropertyChangedEventArgs = new(nameof(stage));
    private static readonly PropertyChangedEventArgs _surePropertyChangedEventArgs = new(nameof(sure));
    private PizzaFilter _filter = default!;
    private Int32 _stage = default!;
    private Boolean? _sure = default;
    public PizzaFilter filter 
    { 
        get => _filter; 
        set
        {
            if(_filter != value)
            {
                _filter = value;
                PropertyChanged?.Invoke(this, _filterPropertyChangedEventArgs);
            }
        }
    }
    public Int32 stage 
    { 
        get => _stage; 
        set
        {
            if(_stage != value)
            {
                _stage = value;
                PropertyChanged?.Invoke(this, _stagePropertyChangedEventArgs);
            }
        }
    }
    public Boolean? sure 
    { 
        get => _sure; 
        set
        {
            if(_sure != value)
            {
                _sure = value;
                PropertyChanged?.Invoke(this, _surePropertyChangedEventArgs);
            }
        }
    }
}