using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class EnumField : UserControl, ICommand, INotifyPropertyChanged
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
    public List<object?> Items { get; private init; } = [];
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
                oldProperty.PropertyChanged -= Property_PropertyChanged;
                Items.Clear();
            }
            if (e.NewValue is Property newProperty)
            {
                ComboBox.DataContext = newProperty;
                UndoButton.Visibility = Property is EntityProperty ep
                    && (ep.Entity.State is EntityState.Unchanged || ep.Entity.State is EntityState.Modified)
                    ? Visibility.Visible : Visibility.Collapsed;
                newProperty.PropertyChanged += Property_PropertyChanged;
                Type enumType = Property.Type;
                if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    enumType = enumType.GetGenericArguments()[0];
                    Items.Add(null);
                }
                if (enumType.IsEnum)
                {
                    foreach(object item in Enum.GetValues(enumType))
                    {
                        Items.Add(item);
                    }
                }
                else
                {
                    Items.Add(true);
                    Items.Add(false);
                }
            }
        }
        base.OnPropertyChanged(e);
    }

    private void Property_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
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
