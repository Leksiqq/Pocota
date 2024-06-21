using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class EnumField : UserControl, ICommand, IFieldOwner
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
        bool res = Field is { } && Field.IsReady
        && (
            "Undo".Equals(parameter)
            || ("Clear".Equals(parameter) && !Field!.IsClean)
        );
        return res;
    }
    public void Execute(object? parameter)
    {
        if (
            Field is { } && Field.IsReady
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
        if(Field is { })
        {
            ComboBox.DataContext = Field;
            UndoButton.Visibility = Field.EntityProperty?.Entity.State is EntityState.Unchanged || Field.EntityProperty?.Entity.State is EntityState.Modified
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
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        ((IFieldOwner)this).FieldOwnerCore!.OnPropertyChanged(e);
        base.OnPropertyChanged(e);
    }
}
