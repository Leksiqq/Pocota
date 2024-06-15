using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class EnumField : UserControl, IValueConverter, ICommand, INotifyPropertyChanged
{
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
       nameof(Property), typeof(Property),
       typeof(EnumField)
    );
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
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    public Property Property
    {
        get => (Property)GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }
    public object? Value
    {
        get => Property?.Value;
        set
        {
            Console.WriteLine("here");
            if (Property is { })
            {
                Console.WriteLine(value);
                Property.Value = value;
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
        }
    }
    public EnumField()
    {
        InitializeComponent();
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("Enum".Equals(parameter))
        {
            List<string> result = [];
            if(Property is { })
            {
                Type enumType = Property.Type;
                if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    enumType = enumType.GetGenericArguments()[0];
                    result.Add(string.Empty);
                }
                if (enumType.IsEnum)
                {
                    result.AddRange(Enum.GetNames(enumType));
                }
                else
                {
                    result.Add(true.ToString());
                    result.Add(false.ToString());
                }
            }
            return result;
        }
        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
    public bool CanExecute(object? parameter)
    {
        bool res = Property is { }
        && (
            "Undo".Equals(parameter)
            || ("Clear".Equals(parameter) && !IsClean())
        );
        return res;
    }
    public void Execute(object? parameter)
    {
        if (
            Property is { }
            && (
                "Undo".Equals(parameter)
                || ("Clear".Equals(parameter) && !IsClean())
            )
        )
        {
            if ("Undo".Equals(parameter))
            {
                //TODO Execute
            }
            else if ("Clear".Equals(parameter))
            {
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
                ComboBox.DataContext = null;
            }
            if (e.NewValue is Property newProperty)
            {
                ComboBox.DataContext = newProperty;
                UndoButton.Visibility = Property is EntityProperty ep
                    && (ep.Entity.State is EntityState.Unchanged || ep.Entity.State is EntityState.Modified)
                    ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        base.OnPropertyChanged(e);
    }
    private void Clear()
    {
        if (Property is { })
        {
            if (Property.IsNullable || Property.Type.IsClass)
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
