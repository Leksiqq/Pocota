using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class ObjectField : UserControl, ICommand, IValueConverter, IServiceRelated, IFieldOwner, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
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
       typeof(ObjectField)
    );
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
       nameof(Target), typeof(object),
       typeof(ObjectField)
    );
    public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
       nameof(PropertyName), typeof(string),
       typeof(ObjectField)
    );
    private static readonly PropertyChangedEventArgs _EditorOpenChangedEventArgs = new(nameof(EditorOpen));
    private static readonly PropertyChangedEventArgs _ObjectStateChangedEventArgs = new(null);
    private ObjectWindow? _editWindow = null;
    private readonly FieldOwnerCore _fieldOwnerCore;
    private string _serviceKey = string.Empty;
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
    public Window Window { get; private set; } = null!;
    public string ServiceKey => _serviceKey;
    public bool EditorOpen => ObjectEditor?.Visibility is Visibility.Visible;
    public ObjectState ObjectState
    {
        get
        {
            if (Field?.EntityProperty?.Entity.Access is Contract.AccessKind.NotSet)
            {
                return ObjectState.NotSet;
            }
            if (Field?.EntityProperty?.Entity.Access is Contract.AccessKind.Forbidden)
            {
                return ObjectState.Forbidden;
            }
            if (Field?.Value is { })
            {
                return ObjectState.IsNotNull;
            }
            return ObjectState.IsNull;
        }
    }
    public ObjectField()
    {
        _fieldOwnerCore = new FieldOwnerCore(this, FieldProperty, TargetProperty, PropertyNameProperty);
        InitializeComponent();
        Loaded += ObjectField_Loaded;
    }
    public bool CanExecute(object? parameter)
    {
        return
            Field is { } && Field.IsReady
            && (
                (
                    Field!.Value is { } 
                    && (
                        "Edit".Equals(parameter)
                        || "EditExternal".Equals(parameter)
                        || "CloseEdit".Equals(parameter)
                    )
                )
                || (
                    !Field.IsReadonly 
                    && Field.Value is null
                    && (
                        (
                            "Find".Equals(parameter) 
                            && typeof(IEntityOwner).IsAssignableFrom(Field.Type)
                        ) 
                        || "Create".Equals(parameter)
                    )
                )
                || (
                    !Field.IsReadonly 
                    && Field.Value is { } 
                    && _editWindow is null
                    && !ObjectEditor.IsVisible
                    && "Clear".Equals(parameter)
                )
            )
            ;
    }
    public void Execute(object? parameter)
    {
        if(Field is { } && Field.IsReady)
        {
            
            if(!Field!.IsReadonly && Field.Value is { } && "Clear".Equals(parameter))
            {
                Field.Value = null;
            }
            else if (Field.Value is { } && "CloseEdit".Equals(parameter))
            {
                ObjectEditor.Visibility = Visibility.Collapsed;
                ObjectEditor.Target = null!;
                PropertyChanged?.Invoke(this, _EditorOpenChangedEventArgs);
            }
            else if(
                (
                    !Field.IsReadonly 
                    && "Create".Equals(parameter)
                )
                || (
                    Field.Value is { } 
                    && (
                        "Edit".Equals(parameter)
                        || "EditExternal".Equals(parameter)
                    )
                )
            )
            {
                if ("Create".Equals(parameter))
                {
                    Field.Value = ((IServiceProvider)FindResource(ServiceProviderResourceKey))
                        .GetRequiredKeyedService<PocotaContext>(ServiceKey).CreateInstance(Field.Type);
                }
                else if ("EditExternal".Equals(parameter))
                {
                    if (Field.IsCollection)
                    {

                    }
                    else
                    {
                        if (_editWindow is null || !_editWindow.IsLoaded)
                        {

                            _editWindow = new ObjectWindow(_serviceKey, Window);
                            WeakEventManager<Window, EventArgs>.AddHandler(_editWindow, "Closed", ExternalEditWindow_Closed);
                            _editWindow.Target = Field.Value;
                            _editWindow.PropertyName = Field.PropertyName;
                            _editWindow.Show();
                        }
                        else
                        {
                            _editWindow.Activate();
                        }
                    }
                }
                else
                {
                    if (Field.IsCollection)
                    {

                    }
                    else
                    {
                        if (Field.Value is IEntityOwner)
                        {
                        }
                        else if (Field.Value is object value)
                        {
                            for(DependencyObject dob = this; dob is not null; dob = VisualTreeHelper.GetParent(dob))
                            {
                                if(dob is ObjectEditor oe)
                                {
                                    ObjectEditor.ServiceProviderCatcher = oe.ServiceProviderCatcher;
                                    break;
                                }
                            }
                            ObjectEditor.Target = value;
                            ObjectEditor.Visibility = Visibility.Visible;
                            PropertyChanged?.Invoke(this, _EditorOpenChangedEventArgs);
                        }
                    }
                }
            }
        }
    }
    private void ExternalEditWindow_Closed(object? sender, EventArgs e)
    {
        if(sender is Window window)
        {
            if(_editWindow == window)
            {
                _editWindow = null;
            }
            WeakEventManager<Window, EventArgs>.RemoveHandler(window, "Closed", ExternalEditWindow_Closed);
        }
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Field is { } && Field.IsReady)
        {
            if ("CountVisibility".Equals(parameter))
            {
                return Field!.IsReady && Field.IsCollection ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        if ("EditVisibility".Equals(parameter))
        {
            return value is bool b && b ? Visibility.Collapsed : Visibility.Visible;
        }
        if ("CloseEditVisibility".Equals(parameter))
        {
            return value is bool b && b ? Visibility.Visible : Visibility.Collapsed;
        }
        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
    public void OnFieldAssigned()
    {
        if(Field is { })
        {
            TextBlock.DataContext = Field;
            CountText.DataContext = Field;
            Find.Visibility = typeof(IEntityOwner).IsAssignableFrom(Field.Type) ? Visibility.Visible : Visibility.Collapsed;
            Field.PropertyChanged += Field_PropertyChanged;
            PropertyChanged?.Invoke(this, _ObjectStateChangedEventArgs);
        }
    }

    private void Field_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, _ObjectStateChangedEventArgs);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        ((IFieldOwner)this).FieldOwnerCore!.OnPropertyChanged(e);
        base.OnPropertyChanged(e);
    }
    private void ObjectField_Loaded(object sender, RoutedEventArgs e)
    {
        for (DependencyObject dop = this; dop is { }; dop = VisualTreeHelper.GetParent(dop))
        {
            if (dop is Window window)
            {
                Window = window;
                if (window is IServiceRelated sr)
                {
                    _serviceKey = sr.ServiceKey;
                }
                break;
            }
        }
    }
}
