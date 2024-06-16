using System.ComponentModel;
using System.Globalization;
using System.Numerics;
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
    private const int s_defaultChangeHeight = 5;
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
       nameof(Property), typeof(Property),
       typeof(TextField)
    );
    public static readonly DependencyProperty ChangeHeightProperty = DependencyProperty.Register(
       nameof(ChangeHeight), typeof(int),
       typeof(TextField)
    );
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private object? _value;
    private string? _badFormat = null;
    private ObjectEditor? _objectEditor = null;
    private int _expectedCaretIndex = -1;
    private double _initialHeight = 0;
    public Property Property
    {
        get => (Property)GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }
    public int ChangeHeight
    {
        get => (int)GetValue(ChangeHeightProperty);
        set => SetValue(ChangeHeightProperty, value);
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
        ChangeHeight = s_defaultChangeHeight;
        InitializeComponent();
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
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
            return ToString(value, Property.Type);
        }
        return value;
    }
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("Text".Equals(parameter))
        {
            if (value is string valueAsString)
            {
                object? res;
                Type type = Property.Type;
                if (Property.IsNullable)
                {
                    if(string.IsNullOrWhiteSpace(valueAsString))
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
                else if (string.IsNullOrWhiteSpace(valueAsString))
                {
                    _badFormat = null;
                    PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
                    return Activator.CreateInstance(type);
                }
                try
                {
                    if(TryParse(valueAsString, type, out res))
                    {
                        _badFormat = null;
                        PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
                        return res;
                    }
                    return BadFormat(valueAsString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return BadFormat(valueAsString);
                }
            }
        }
        return value;
    }
    public bool CanExecute(object? parameter)
    {
        bool res = Property is { } 
        && (
            "Undo".Equals(parameter)
            || "Increase".Equals(parameter)
            || (
                "Decrease".Equals(parameter)
                && (
                    _initialHeight >= TextBox.FontSize
                    || (TextBox.ActualHeight - _initialHeight >= TextBox.FontSize * ChangeHeight)
                )
            )
            || ("Clear".Equals(parameter) && !IsClean())
        );
        return res;
    }
    public void Execute(object? parameter)
    {
        if(
            Property is { }
            && (
                "Undo".Equals(parameter)
                || "Increase".Equals(parameter)
                || (
                    "Decrease".Equals(parameter) 
                    && (
                        _initialHeight >= TextBox.FontSize
                        || (TextBox.ActualHeight - _initialHeight >= TextBox.FontSize * ChangeHeight)
                    )
                )
                || ("Clear".Equals(parameter) && !IsClean())
            )
        )
        {
            if("Undo".Equals(parameter))
            {
                //TODO Execute
            }
            else if ("Clear".Equals(parameter))
            {
                _badFormat = null;
                Clear();
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
            else if ("Increase".Equals(parameter) || "Decrease".Equals(parameter))
            {
                if ("Increase".Equals(parameter))
                {
                    if(_initialHeight <= 0.1) 
                    {
                        _initialHeight = TextBox.ActualHeight;
                    }
                    TextBox.Height = TextBox.ActualHeight + TextBox.FontSize * ChangeHeight;
                }
                else if ("Decrease".Equals(parameter))
                {
                    if(TextBox.ActualHeight - _initialHeight >= TextBox.FontSize * ChangeHeight)
                    {
                        TextBox.Height = TextBox.ActualHeight - TextBox.FontSize * ChangeHeight;
                    }
                    else
                    {
                        TextBox.Height = _initialHeight;
                    }
                }
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
                UndoButton.Visibility = Property is EntityProperty ep 
                    && (ep.Entity.State is EntityState.Unchanged || ep.Entity.State is EntityState.Modified)
                    ? Visibility.Visible : Visibility.Collapsed;
                IncreaseTextButton.Visibility = Property.Type == typeof(string) ? Visibility.Visible : Visibility.Collapsed;
                DecreaseTextButton.Visibility = Property.Type == typeof(string) ? Visibility.Visible : Visibility.Collapsed;
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
    private static string? ToString(object value, Type type)
    {
        if (value is { })
        {
            return value.ToString();
        }
        return string.Empty;
    }
    private static bool TryParse(string valueAsString, Type type, out object? res)
    {
        if (type == typeof(DateOnly))
        {
            if (DateOnly.TryParse(valueAsString, out DateOnly don))
            {
                res = don;
                return true;
            }
            res = null;
            return false;
        }
        if (type == typeof(TimeOnly))
        {
            if (TimeOnly.TryParse(valueAsString, CultureInfo.CurrentCulture.DateTimeFormat, out TimeOnly ton))
            {
                res = ton;
                return true;
            }
            res = null;
            return false;
        }
        if (type == typeof(TimeSpan))
        {
            if (TimeSpan.TryParse(valueAsString, out TimeSpan ts))
            {
                res = ts;
                return true;
            }
            res = null;
            return false;
        }
        res = System.Convert.ChangeType(valueAsString, type);
        if (IsNumeric(res) && res.ToString() != valueAsString)
        {
            return false;
        }
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
        if(sender is TextBox tb)
        {
            if (e.Key is Key.Insert)
            {
                IsInsertMode = !IsInsertMode;
                if (Editor is { })
                {
                    Editor.CurrentInput = this;
                }
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
    private static bool IsNumeric(object value)
    {
        return (value is Byte ||
                value is Int16 ||
                value is Int32 ||
                value is Int64 ||
                value is SByte ||
                value is UInt16 ||
                value is UInt32 ||
                value is UInt64 ||
                value is BigInteger ||
                value is Decimal ||
                value is Double ||
                value is Single);
    }
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(sender is TextBox tb)
        {
            if(_expectedCaretIndex != -1 && _expectedCaretIndex != tb.CaretIndex)
            {
                tb.CaretIndex = _expectedCaretIndex;
            }
            _expectedCaretIndex = -1;
        }
    }
    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is TextBox tb)
        {
            if (!string.IsNullOrEmpty(e.TextComposition.Text))
            {
                _expectedCaretIndex = tb.CaretIndex + e.TextComposition.Text.Length;
            }
        }
    }
}
