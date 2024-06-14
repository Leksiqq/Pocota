using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class TextField : UserControl, IValueConverter, INotifyPropertyChanged, ICommand, IInputElement
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
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private object? _value;
    private string? _badFormat = null;
    private ObjectEditor? _objectEditor = null;
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
    public bool IsInsertMode { get; private set; } = true;
    private ObjectEditor? Editor
    {
        get
        {
            if (_objectEditor is null)
            {
                if (GetAncestors(this).OfType<ObjectEditor>().FirstOrDefault() is ObjectEditor oe)
                {
                    _objectEditor = oe;
                }
            }
            return _objectEditor;
        }
    }
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
                object? res;
                Type type = Property.Type;
                if (Property.IsNullable)
                {
                    if(string.IsNullOrWhiteSpace(s))
                    {
                        _badFormat = null;
                        PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
                        return null;
                    }
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        type = type.GetGenericArguments()[0];
                    }
                }
                try
                {
                    if(TryParse(s, type, out res))
                    {
                        _badFormat = null;
                        PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
                        return res;
                    }
                    return BadFormat(s);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return BadFormat(s);
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
            }
            if (e.NewValue is Property newProperty)
            {
                TextBox.DataContext = this;
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
    private bool TryParse(string s, Type type, out object? res)
    {
        if (type == typeof(DateOnly))
        {
            if (DateOnly.TryParse(s, out DateOnly don))
            {
                res = don;
                return true;
            }
            res = null;
            return false;
        }
        if (type == typeof(TimeOnly))
        {
            if (TimeOnly.TryParse(s, CultureInfo.CurrentCulture.DateTimeFormat, out TimeOnly ton))
            {
                res = ton;
                return true;
            }
            res = null;
            return false;
        }
        if (type == typeof(TimeSpan))
        {
            if (TimeSpan.TryParse(s, out TimeSpan ts))
            {
                res = ts;
                return true;
            }
            res = null;
            return false;
        }
        res = System.Convert.ChangeType(s, type);
        return true;
    }
    private object? BadFormat(string s)
    {
        _badFormat = s;
        PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
        Console.WriteLine($"_badFormat: {_badFormat}");
        return _value;
    }

    private void TextBox_KeyUp(object sender, KeyEventArgs e)
    {
        if(e.Key is Key.Insert)
        {
            IsInsertMode = !IsInsertMode;
            if(Editor is { })
            {
                Editor.CurrentInput = this;
            }
        }
    }
    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if(sender is TextBox tb && Editor is { })
        {
            Editor.CurrentInput = this;
        }
    }
    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb && Editor is { })
        {
            Editor.CurrentInput = null;
        }
    }

    private static IEnumerable<DependencyObject> GetAncestors(DependencyObject obj)
    {
        for (var cur = obj; cur is not null; cur = VisualTreeHelper.GetParent(cur))
        {
            yield return cur;
        }
    }
}
