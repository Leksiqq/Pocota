/////////////////////////////////////////////////////////////
// ContosoPizza.Client.PizzaFilter                         //
// was generated automatically from ContosoPizza.IContract //
// at 2024-06-19T17:22:13.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using ContosoPizza.Models;
using ContosoPizza.Models.Client;
using Net.Leksi.Pocota.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContosoPizza.Client;


public class PizzaFilter: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _ToppingsPropertyChangedEventArgs = new(nameof(Toppings));
    private static readonly PropertyChangedEventArgs _SaucesPropertyChangedEventArgs = new(nameof(Sauces));
    private static readonly PropertyChangedEventArgs _NameRegexPropertyChangedEventArgs = new(nameof(NameRegex));
    private static readonly PropertyChangedEventArgs _TagsPropertyChangedEventArgs = new(nameof(Tags));
    private static readonly PropertyChangedEventArgs _DecsPropertyChangedEventArgs = new(nameof(Decs));
    private static readonly PropertyChangedEventArgs _DateTimePropertyChangedEventArgs = new(nameof(DateTime));
    private static readonly PropertyChangedEventArgs _DateOnlyPropertyChangedEventArgs = new(nameof(DateOnly));
    private static readonly PropertyChangedEventArgs _TimeSpanPropertyChangedEventArgs = new(nameof(TimeSpan));
    private static readonly PropertyChangedEventArgs _AccessKindPropertyChangedEventArgs = new(nameof(AccessKind));
    private static readonly PropertyChangedEventArgs _CanSingPropertyChangedEventArgs = new(nameof(CanSing));
    private static readonly PropertyChangedEventArgs _BoolsPropertyChangedEventArgs = new(nameof(Bools));
    private static readonly PropertyChangedEventArgs _NotNullIntPropertyChangedEventArgs = new(nameof(NotNullInt));
    private static readonly PropertyChangedEventArgs _DoubleValuePropertyChangedEventArgs = new(nameof(DoubleValue));
    private readonly ObservableCollection<Topping> _Toppings = [];
    private readonly ObservableCollection<Sauce> _Sauces = [];
    private String? _NameRegex = default;
    private readonly ObservableCollection<String> _Tags = [];
    private readonly ObservableCollection<Decimal> _Decs = [];
    private DateTime? _DateTime = default;
    private DateOnly? _DateOnly = default;
    private TimeSpan? _TimeSpan = default;
    private AccessKind? _AccessKind = default;
    private Boolean? _CanSing = default;
    private readonly ObservableCollection<Boolean> _Bools = [];
    private Int32 _NotNullInt = default!;
    private Double _DoubleValue = default!;
    public ObservableCollection<Topping>? Toppings 
    { 
        get => _Toppings; 
    }
    public ObservableCollection<Sauce>? Sauces 
    { 
        get => _Sauces; 
    }
    public String? NameRegex 
    { 
        get => _NameRegex; 
        set
        {
            if(_NameRegex != value)
            {
                _NameRegex = value;
                PropertyChanged?.Invoke(this, _NameRegexPropertyChangedEventArgs);
            }
        }
    }
    public ObservableCollection<String>? Tags 
    { 
        get => _Tags; 
    }
    public ObservableCollection<Decimal>? Decs 
    { 
        get => _Decs; 
    }
    public DateTime? DateTime 
    { 
        get => _DateTime; 
        set
        {
            if(_DateTime != value)
            {
                _DateTime = value;
                PropertyChanged?.Invoke(this, _DateTimePropertyChangedEventArgs);
            }
        }
    }
    public DateOnly? DateOnly 
    { 
        get => _DateOnly; 
        set
        {
            if(_DateOnly != value)
            {
                _DateOnly = value;
                PropertyChanged?.Invoke(this, _DateOnlyPropertyChangedEventArgs);
            }
        }
    }
    public TimeSpan? TimeSpan 
    { 
        get => _TimeSpan; 
        set
        {
            if(_TimeSpan != value)
            {
                _TimeSpan = value;
                PropertyChanged?.Invoke(this, _TimeSpanPropertyChangedEventArgs);
            }
        }
    }
    public AccessKind? AccessKind 
    { 
        get => _AccessKind; 
        set
        {
            if(_AccessKind != value)
            {
                _AccessKind = value;
                PropertyChanged?.Invoke(this, _AccessKindPropertyChangedEventArgs);
            }
        }
    }
    public Boolean? CanSing 
    { 
        get => _CanSing; 
        set
        {
            if(_CanSing != value)
            {
                _CanSing = value;
                PropertyChanged?.Invoke(this, _CanSingPropertyChangedEventArgs);
            }
        }
    }
    public ObservableCollection<Boolean>? Bools 
    { 
        get => _Bools; 
    }
    public Int32 NotNullInt 
    { 
        get => _NotNullInt; 
        set
        {
            if(_NotNullInt != value)
            {
                _NotNullInt = value;
                PropertyChanged?.Invoke(this, _NotNullIntPropertyChangedEventArgs);
            }
        }
    }
    public Double DoubleValue 
    { 
        get => _DoubleValue; 
        set
        {
            if(_DoubleValue != value)
            {
                _DoubleValue = value;
                PropertyChanged?.Invoke(this, _DoubleValuePropertyChangedEventArgs);
            }
        }
    }
}