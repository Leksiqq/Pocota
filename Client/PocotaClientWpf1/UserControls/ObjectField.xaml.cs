using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client.UserControls;
public partial class ObjectField : UserControl, ICommand, IValueConverter, IServiceRelated, INotifyPropertyChanged
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
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
       nameof(Target), typeof(object),
       typeof(TextField)
    );
    public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
       nameof(PropertyName), typeof(string),
       typeof(TextField)
    );
    public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(
       nameof(Window), typeof(Window),
       typeof(ObjectField)
    );
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private readonly WeakReference<ObjectWindow> _editWindow = new(null!);
    private string _serviceKey = string.Empty;
    private PropertyInfo? _propertyInfo = null;
    private NullabilityInfoContext _nullability = new();
    private bool _isNullable = false;
    private bool _isCollection = false;
    private EntityProperty? _entityProperty;
    private bool IsAssigned => Target is { } && PropertyName is { };
    public bool IsReadonly => _entityProperty?.IsReadonly ?? false;
    public object Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }
    public string PropertyName
    {
        get => (string)GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }
    public Window Window
    {
        get => (Window)GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }
    public string ServiceKey => _serviceKey;
    public int Count => IsAssigned && _isCollection ? (Value as IList)?.Count ?? 0 : 0;
    public bool EditorOpen => ObjectEditor?.Visibility is Visibility.Visible;
    public object? Value
    {
        get => IsAssigned ? _propertyInfo!.GetValue(Target) : null;
        set
        {
            if (IsAssigned)
            {
                _propertyInfo!.SetValue(Target, value);
            }
        }
    }
    public Type Type => IsAssigned ? _propertyInfo?.PropertyType ?? typeof(object) : typeof(object);
    public ObjectField()
    {
        InitializeComponent();
    }
    public bool CanExecute(object? parameter)
    {
        return 
            IsAssigned 
            && (
                (
                    Value is { } 
                    && (
                        "Edit".Equals(parameter)
                        || "EditExternal".Equals(parameter)
                        || "CloseEdit".Equals(parameter)
                    )
                )
                || (
                    !IsReadonly 
                    && Value is null
                    && (
                        (
                            "Find".Equals(parameter) 
                            && typeof(IEntityOwner).IsAssignableFrom(Type)
                        ) 
                        || "Create".Equals(parameter)
                    )
                )
                || (!IsReadonly && Value is { } && "Clear".Equals(parameter))
            )
            ;
    }
    public void Execute(object? parameter)
    {
        if(IsAssigned)
        {
            
            if(!IsReadonly && Value is { } && "Clear".Equals(parameter))
            {
                CloseEdit();
                Value = default;
            }
            else if (Value is { } && "CloseEdit".Equals(parameter))
            {
                CloseEdit();
            }
            else if(
                (
                    !IsReadonly 
                    && "Create".Equals(parameter)
                )
                || (
                    Value is { } 
                    && (
                        "Edit".Equals(parameter)
                        || "EditExternal".Equals(parameter)
                    )
                )
            )
            {
                if ("Create".Equals(parameter))
                {
                    Value = ((IServiceProvider)FindResource(ServiceProviderResourceKey))
                        .GetRequiredKeyedService<PocotaContext>(ServiceKey).CreateInstance(Type);
                }
                else if ("EditExternal".Equals(parameter))
                {
                    if (_isCollection)
                    {

                    }
                    else
                    {
                        if (!_editWindow.TryGetTarget(out ObjectWindow? window) || !window.IsLoaded)
                        {
                            
                            window = new ObjectWindow(_serviceKey, Window);
                            _editWindow.SetTarget(window);
                            window.Target = Target;
                            window.Show();
                        }
                        else
                        {
                            window.Activate();
                        }
                    }
                }
                else
                {
                    if (_isCollection)
                    {

                    }
                    else
                    {
                        if (Value is IEntityOwner eo)
                        {
                        }
                        else if (Value is object value)
                        {
                            for(DependencyObject dob = this; dob is not null; dob = VisualTreeHelper.GetParent(dob))
                            {
                                if(dob is ObjectEditor oe)
                                {
                                    if(_editWindow is { } && _editWindow.TryGetTarget(out ObjectWindow? window) && window.IsLoaded)
                                    {
                                        ObjectEditor.Target = value;
                                    }
                                    else
                                    {
                                        ObservableCollection<Property> properties = [];
                                        foreach (PropertyInfo pi in Property.Type.GetProperties())
                                        {
                                            properties.Add(Property.Create(pi, Property.Value)!);
                                        }
                                        ObjectEditor.Properties = properties;
                                    }
                                    ObjectEditor.Window = Window;
                                    ObjectEditor.ServiceProviderCatcher = oe.ServiceProviderCatcher;
                                    ObjectEditor.Visibility = Visibility.Visible;
                                    PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(IsAssigned)
        {
            if ("ObjectState".Equals(parameter))
            {
                if (_entityProperty?.Entity.Access is Contract.AccessKind.NotSet)
                {
                    return ObjectState.NotSet;
                }
                if (_entityProperty?.Entity.Access is Contract.AccessKind.Forbidden)
                {
                    return ObjectState.Forbidden;
                }
                if (_propertyInfo?.GetValue(Target) is { })
                {  
                    return ObjectState.IsNotNull;
                }
                return ObjectState.IsNull;
            }
            if ("CountVisibility".Equals(parameter))
            {
                return IsAssigned && _isCollection ? Visibility.Visible : Visibility.Collapsed;
            }
            if ("EditVisibility".Equals(parameter))
            {
                return value is bool b && b ? Visibility.Collapsed : Visibility.Visible;
            }
            if ("CloseEditVisibility".Equals(parameter))
            {
                return value is bool b && b ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == TargetProperty)
        {
            if (e.OldValue is { })
            {
                TextBlock.DataContext = null;
            }
            if (e.NewValue is { })
            {
                TextBlock.DataContext = e.NewValue;
            }
            ProcessProperties();
        }
        else if (e.Property == PropertyNameProperty)
        {
            ProcessProperties();
        }
        else if (e.Property == WindowProperty)
        {
            if (e.OldValue is Window)
            {
                _serviceKey = string.Empty;
            }
            if (e.NewValue is Window)
            {
                if (e.NewValue is IServiceRelated sr)
                {
                    _serviceKey = sr.ServiceKey;
                }
            }
        }
        base.OnPropertyChanged(e);
    }

    private void ProcessProperties()
    {
        if (IsAssigned)
        {
            _entityProperty = Target is IEntityOwner eo ? eo.Entity.GetEntityProperty(PropertyName) : null;
            _propertyInfo = Target.GetType().GetProperty(PropertyName);
            _isNullable = _nullability.Create(_propertyInfo!).ReadState is NullabilityState.Nullable;
            _isCollection = Type.IsGenericType && typeof(ObservableCollection<>).IsAssignableFrom(Type.GetGenericTypeDefinition());
            Find.Visibility = typeof(IEntityOwner).IsAssignableFrom(Type) ? Visibility.Visible : Visibility.Collapsed;
            PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
        }
    }

    private void CloseEdit()
    {
        if (ObjectEditor.Visibility is Visibility.Visible)
        {
            ObjectEditor.Visibility = Visibility.Collapsed;
            ObjectEditor.Target = null!;
            PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
        }
    }
    private void Property_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
    }
}
