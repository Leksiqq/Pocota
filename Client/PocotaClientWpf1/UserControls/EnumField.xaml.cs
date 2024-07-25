using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class EnumField : UserControl, ICommand, IFieldOwner, IValueConverter
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
    public static readonly DependencyProperty FieldProperty = DependencyProperty.Register(
       nameof(Field), typeof(Field),
       typeof(EnumField)
    );
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
       nameof(Target), typeof(object),
       typeof(EnumField)
    );
    public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
       nameof(PropertyName), typeof(string),
       typeof(EnumField)
    );
    private readonly FieldOwnerCore _fieldOwnerCore;
    private bool _ignoreValueChanged = false;
    FieldOwnerCore IFieldOwner.FieldOwnerCore => _fieldOwnerCore;
    public Field? Field
    {
        get => (Field?)GetValue(FieldProperty);
        set => SetValue(FieldProperty, value);
    }
    public object? Target
    {
        get => (object?)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }
    public string? PropertyName
    {
        get => (string?)GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }
    public List<object?> Items { get; private init; } = [];
    public EnumField()
    {
        _fieldOwnerCore = new FieldOwnerCore(this, FieldProperty, TargetProperty, PropertyNameProperty);
        InitializeComponent();
    }
    public bool CanExecute(object? parameter)
    {
        bool res = Field != null && Field.IsReady
        && (
            "Undo".Equals(parameter)
            || ("Clear".Equals(parameter) && !Field!.IsClean)
        );
        return res;
    }
    public void Execute(object? parameter)
    {
        if (
            Field != null && Field.IsReady
            && (
                "Undo".Equals(parameter)
                || ("Clear".Equals(parameter) && !Field!.IsClean)
            )
        )
        {
            if ("Undo".Equals(parameter))
            {
                //TODO Execute
            }
            else if ("Clear".Equals(parameter))
            {
                Field!.Clear();
            }
        }
    }
    public void OnFieldAssigned()
    {
        if(Field != null)
        {
            ComboBox.DataContext = Field;
            UndoButton.Visibility = Field.EntityProperty?.Entity.State == EntityState.Unchanged || Field.EntityProperty?.Entity.State == EntityState.Modified
                ? Visibility.Visible : Visibility.Collapsed;
            if (Field.IsNullable)
            {
                Items.Add(null);
            }
            if (Field.Type.IsEnum)
            {
                foreach (object item in Enum.GetValues(Field.Type))
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
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("Value".Equals(parameter))
        {
            _ignoreValueChanged = false;
        }
        return value;
    }
    public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if("Value".Equals(parameter))
        {
            _ignoreValueChanged = true;
        }
        return value;
    }
    public void OnValueChanged()
    {
        if(!_ignoreValueChanged)
        {
            ComboBox.SelectedItem = Field?.Value;
        }
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        ((IFieldOwner)this).FieldOwnerCore!.OnPropertyChanged(e);
        base.OnPropertyChanged(e);
    }
}
