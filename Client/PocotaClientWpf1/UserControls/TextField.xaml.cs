using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class TextField : UserControl, IValueConverter, INotifyPropertyChanged, ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
       nameof(Property), typeof(Property),
       typeof(TextField)
    );
    private object? _value;
    private string? _badFormat = null;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    public Property Property
    {
        get => (Property)GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }
    public object? Value
    {
        get => Property?.Value;
        set {
            if(Property is { })
            {
                Property.Value = value;
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
        }
    }
    
    public bool IsReadonly => Property?.IsReadonly ?? true;
    public TextField()
    {
        InitializeComponent();
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine($"{value}, {parameter}");
        if ("Foreground".Equals(parameter))
        {
            return _badFormat is { } ? Brushes.Red : Brushes.Black;
        }
        if ("Text".Equals(parameter))
        {
            _value = value;
            if(_badFormat is { })
            {
                return _badFormat;
            }
            if (value is string s)
            {
                return s;
            }
            if (value is Decimal d)
            {
                return d.ToString(CultureInfo.CurrentCulture.NumberFormat);
            }
            if (value is DateTime dt)
            {
                return dt.ToString(CultureInfo.CurrentCulture.DateTimeFormat);
            }
            if (value is DateOnly don)
            {
                return don.ToString(CultureInfo.CurrentCulture.DateTimeFormat);
            }
            if (value is TimeOnly ton)
            {
                return ton.ToString(CultureInfo.CurrentCulture.DateTimeFormat);
            }
            if(value is { })
            {
                return value.ToString();
            }
            return string.Empty;
        }
        return value;
    }
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("Text".Equals(parameter))
        {
            if (value is string s)
            {
                Console.WriteLine($"value: \"{s}\"");
                try
                {
                    object? res = System.Convert.ChangeType(s, Property.Type);
                    _badFormat = null;
                    PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
                    Console.WriteLine($"_badFormat: {_badFormat}");
                    return res;
                }
                catch (FormatException)
                {
                    _badFormat = s;
                    PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
                    Console.WriteLine($"_badFormat: {_badFormat}");
                    return _value;
                }
            }
        }
        return value;
    }
    public bool CanExecute(object? parameter)
    {
        bool res = Property is { } 
        && (
            ("Undo".Equals(parameter) && _badFormat is { }) 
            || ("Clear".Equals(parameter) && !IsClean())
        );
        return res;
    }
    public void Execute(object? parameter)
    {
        if(
            Property is { }
            && (
                ("Undo".Equals(parameter) && _badFormat is { })
                || ("Clear".Equals(parameter) && !IsClean())
            )
        )
        {
            if("Undo".Equals(parameter))
            {
                _badFormat = null;
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
            else if ("Clear".Equals(parameter))
            {
                _badFormat = null;
                Clear();
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
        }
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == PropertyProperty)
        {
            if (e.OldValue is Property oldProperty)
            {
                TextBox.DataContext = null;
                TextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            if (e.NewValue is Property newProperty)
            {
                TextBox.DataContext = this;
                TextBox.VerticalScrollBarVisibility = Property.Type == typeof(string) ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            }
        }
        base.OnPropertyChanged(e);
    }
    private void Clear()
    {
        if(Property is { })
        {
            if(Property.IsNullable || Property.Type.IsClass)
            {
                Value = null;
            }
            else
            {
                Value = Activator.CreateInstance(Property.Type);
            }
        }
    }
    private bool IsClean()
    {
        if (Property is { })
        {
            if (Property.IsNullable || Property.Type.IsClass)
            {
                return Value is null;
            }
            return Value?.Equals(Activator.CreateInstance(Property.Type)) ?? true;
        }
        return true;
    }
}
