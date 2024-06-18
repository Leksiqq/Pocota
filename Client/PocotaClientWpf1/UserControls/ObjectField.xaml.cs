using Microsoft.Extensions.DependencyInjection;
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
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
       nameof(Property), typeof(Property),
       typeof(ObjectField)
    );
    public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(
       nameof(Window), typeof(Window),
       typeof(ObjectField)
    );
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private string _serviceKey = string.Empty;
    private readonly WeakReference<ObjectWindow> _editWindow = new(null!);
    public Property Property
    {
        get => (Property)GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }
    public Window Window
    {
        get => (Window)GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }
    public string ServiceKey => _serviceKey;
    public int Count => Property is ListProperty lp ? lp.Count : 0;
    public bool EditorOpen => ObjectEditor?.Visibility is Visibility.Visible;
    public ObjectField()
    {
        InitializeComponent();
    }
    public bool CanExecute(object? parameter)
    {
        return 
            Property is { } 
            && (
                (
                    Property.Value is { } 
                    && (
                        "Edit".Equals(parameter)
                        || "EditExternal".Equals(parameter)
                        || "CloseEdit".Equals(parameter)
                    )
                )
                || (
                    !Property.IsReadonly 
                    && Property.Value is null
                    && (
                        (
                            "Find".Equals(parameter) 
                            && typeof(IEntityOwner).IsAssignableFrom(Property.Type)
                        ) 
                        || "Create".Equals(parameter)
                    )
                )
                || (!Property.IsReadonly && Property.Value is { } && "Clear".Equals(parameter))
            )
            ;
    }
    public void Execute(object? parameter)
    {
        if(Property is { })
        {
            
            if(!Property.IsReadonly && Property.Value is { } && "Clear".Equals(parameter))
            {
                CloseEdit();
                Property.Value = default;
            }
            else if (Property.Value is { } && "CloseEdit".Equals(parameter))
            {
                CloseEdit();
            }
            else if(
                (
                    !Property.IsReadonly 
                    && "Create".Equals(parameter)
                )
                || (
                    Property.Value is { } 
                    && (
                        "Edit".Equals(parameter)
                        || "EditExternal".Equals(parameter)
                    )
                )
            )
            {
                if ("Create".Equals(parameter))
                {
                    Property.Value = ((IServiceProvider)FindResource(ServiceProviderResourceKey))
                        .GetRequiredKeyedService<PocotaContext>(ServiceKey).CreateInstance(Property.Type);
                }
                else if ("EditExternal".Equals(parameter))
                {
                    if (Property is ListProperty)
                    {

                    }
                    else
                    {
                        if (!_editWindow.TryGetTarget(out ObjectWindow? window) || !window.IsLoaded)
                        {
                            
                            window = new ObjectWindow(_serviceKey, Window);
                            _editWindow.SetTarget(window);
                            if(ObjectEditor.Visibility is Visibility.Visible)
                            {
                                foreach(Property property in ObjectEditor.Properties)
                                {
                                    window.Properties.Add(property);
                                }
                            }
                            else
                            {
                                window.Property = Property;
                            }
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
                    if (Property is ListProperty)
                    {

                    }
                    else
                    {
                        if (Property.Value is IEntityOwner eo)
                        {
                        }
                        else if (Property.Value is { })
                        {
                            for(DependencyObject dob = this; dob is not null; dob = VisualTreeHelper.GetParent(dob))
                            {
                                if(dob is ObjectEditor oe)
                                {
                                    if(_editWindow is { } && _editWindow.TryGetTarget(out ObjectWindow? window) && window.IsLoaded)
                                    {
                                        ObjectEditor.Properties = window.Properties;
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
        if(Property is {})
        {
            if ("ObjectState".Equals(parameter))
            {
                if (Property.Access is Contract.AccessKind.NotSet)
                {
                    return ObjectState.NotSet;
                }
                if (Property.Access is Contract.AccessKind.Forbidden)
                {
                    return ObjectState.Forbidden;
                }
                if (Property.Value is { })
                {  
                    return ObjectState.IsNotNull;
                }
                return ObjectState.IsNull;
            }
            if ("CountVisibility".Equals(parameter))
            {
                return Property is ListProperty && Property.Value is { } ? Visibility.Visible : Visibility.Collapsed;
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
        if (e.Property == PropertyProperty)
        {
            if (e.OldValue is Property oldProperty)
            {
                TextBlock.DataContext = null;
                Find.Visibility = Visibility.Collapsed;
                CountText.Visibility = Visibility.Collapsed;
                oldProperty.PropertyChanged -= Property_PropertyChanged;
            }
            if (e.NewValue is Property newProperty)
            {
                TextBlock.DataContext = newProperty;
                Find.Visibility = typeof(IEntityOwner).IsAssignableFrom(Property.Type) ? Visibility.Visible : Visibility.Collapsed;
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
                newProperty.PropertyChanged += Property_PropertyChanged;
            }
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
    private void CloseEdit()
    {
        if (ObjectEditor.Visibility is Visibility.Visible)
        {
            ObjectEditor.Visibility = Visibility.Collapsed;
            ObjectEditor.Properties = null!;
            PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
        }
    }
    private void Property_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
    }
}
