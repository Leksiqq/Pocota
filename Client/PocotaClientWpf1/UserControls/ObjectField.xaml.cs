using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client.UserControls;
public partial class ObjectField : UserControl, ICommand, IValueConverter, IServiceRelated, IFieldOwner
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
       nameof(Field), typeof(IField),
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
    public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(
       nameof(Window), typeof(Window),
       typeof(ObjectField)
    );
    private readonly WeakReference<ObjectWindow> _editWindow = new(null!);
    private string _serviceKey = string.Empty;
    private IField.WaitingForFlags _waitingFor = IField.WaitingForFlags.Any;
    public IField? Field
    {
        get => (IField?)GetValue(FieldProperty);
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
    public Window Window
    {
        get => (Window)GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }
    public string ServiceKey => _serviceKey;
    public int Count => Field?.IsReady ?? false && Field.IsCollection ? (Field.Value as IList)?.Count ?? 0 : 0;
    public bool EditorOpen => ObjectEditor?.Visibility is Visibility.Visible;
    public ObjectField()
    {
        InitializeComponent();
    }
    public bool CanExecute(object? parameter)
    {
        Console.WriteLine($"CanExecute: {parameter}, {JsonSerializer.Serialize(Field)}");
        return 
            Field?.IsReady ?? false
            && (
                (
                    Field.Value is { } 
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
                || (!Field.IsReadonly && Field.Value is { } && "Clear".Equals(parameter))
            )
            ;
    }
    public void Execute(object? parameter)
    {
        if(Field?.IsReady ?? false)
        {
            
            if(!Field.IsReadonly && Field.Value is { } && "Clear".Equals(parameter))
            {
                CloseEdit();
                Field.Value = null;
            }
            else if (Field.Value is { } && "CloseEdit".Equals(parameter))
            {
                CloseEdit();
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
                        if (!_editWindow.TryGetTarget(out ObjectWindow? window) || !window.IsLoaded)
                        {
                            
                            window = new ObjectWindow(_serviceKey, Window);
                            _editWindow.SetTarget(window);
                            window.Target = Field.Target;
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
                    if (Field.IsCollection)
                    {

                    }
                    else
                    {
                        if (Field.Value is IEntityOwner eo)
                        {
                        }
                        else if (Field.Value is object value)
                        {
                            for(DependencyObject dob = this; dob is not null; dob = VisualTreeHelper.GetParent(dob))
                            {
                                if(dob is ObjectEditor oe)
                                {
                                    ObjectEditor.Target = value;
                                    ObjectEditor.Window = Window;
                                    ObjectEditor.ServiceProviderCatcher = oe.ServiceProviderCatcher;
                                    ObjectEditor.Visibility = Visibility.Visible;
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
        if(Field?.IsReady ?? false)
        {
            if ("ObjectState".Equals(parameter))
            {
                if (Field.EntityProperty?.Entity.Access is Contract.AccessKind.NotSet)
                {
                    return ObjectState.NotSet;
                }
                if (Field.EntityProperty?.Entity.Access is Contract.AccessKind.Forbidden)
                {
                    return ObjectState.Forbidden;
                }
                if (Field.Value is { })
                {  
                    return ObjectState.IsNotNull;
                }
                return ObjectState.IsNull;
            }
            if ("CountVisibility".Equals(parameter))
            {
                return Field.IsReady && Field.IsCollection ? Visibility.Visible : Visibility.Collapsed;
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
    public void OnFieldAssigned()
    {
        if(Field is { })
        {
            TextBlock.DataContext = Field;
            Find.Visibility = typeof(IEntityOwner).IsAssignableFrom(Field.Type) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    public void OnFieldDeassigned()
    {
        TextBlock.DataContext = null;
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == FieldProperty)
        {
            if (IField.CanProcessProperty(_waitingFor, IField.WaitingForFlags.Field))
            {
                if (e.NewValue is IField newField)
                {
                    newField.Owner = this;
                }
            }
        }
        else if (e.Property == PropertyNameProperty)
        {
            if (IField.CanProcessProperty(_waitingFor, IField.WaitingForFlags.PropertyName))
            {
                if (_waitingFor is IField.WaitingForFlags.None)
                {
                    Field = new Field { Target = Target, PropertyName = PropertyName, Owner = this };
                }
            }
        }
        else if (e.Property == TargetProperty)
        {
            if (IField.CanProcessProperty(_waitingFor, IField.WaitingForFlags.Target))
            {
                if (_waitingFor is IField.WaitingForFlags.None)
                {
                    Field = new Field { Target = Target, PropertyName = PropertyName, Owner = this };
                }
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
            ObjectEditor.Target = null!;
        }
    }
}
