using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;
public partial class EditList : Window, INotifyPropertyChanged, IEditWindow, ICommand
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
    private readonly ConditionalWeakTable<object, object> _indexMapping = [];
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    private object? _value;
    private Window? _launchedBy;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    public DataGridManager ItemsDataGridManager { get; private init; } = new();
    public WindowsList Windows { get; private init; }
    public bool IsReadonly { get; private init; }
    public Type ItemType { get; private set; }
    public Window? LaunchedBy
    {
        get => _launchedBy;
        set
        {
            if (_launchedBy != value)
            {
                _launchedBy = value;
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
        }
    }
    public EditWindowCore EditWindowCore { get; private init; }
    public object? Value
    {
        get => _value;
        set
        {
            if (_value != value && value is { })
            {
                if (
                    value.GetType().IsGenericType
                    && value.GetType().GetGenericArguments()[0] is Type itemType
                    && typeof(IList<>).MakeGenericType([itemType]).IsAssignableFrom(value.GetType())
                )
                {
                    ItemType = itemType;
                    _value = value;
                    DataGrid.Columns.Clear();
                    DataGridTemplateColumn column;
                    column = new DataGridTemplateColumn();
                    column.CellTemplate = DataGrid.Resources["Position"] as DataTemplate;
                    column.Header = (Application.Current.Resources["I18nConverter"] as I18nConverter)?
                        .Convert("POSITION", typeof(string), null!, CultureInfo.InvariantCulture);
                    DataGrid.Columns.Add(column);
                    if (itemType == typeof(string) || !itemType.IsClass)
                    {
                        column = new DataGridTemplateColumn();
                        ParameterizedResourceExtension pre = new("Field")
                        {
                            Replaces = new string[] { "$field:." }
                        };
                        column.CellTemplate = pre.ProvideValue((FindResource("SPC") as XamlServiceProviderCatcher)!.ServiceProvider!) as DataTemplate;
                        DataGridConverter converter = new()
                        {
                            DataGridManager = ItemsDataGridManager,
                            FieldName = string.Empty
                        };
                        Resources.Add("ValueConverter", converter);
                        pre = new("SortHeader1")
                        {
                            Replaces = new string[] {
                                "$converter:ValueConverter",
                                "$field:",
                                $"$name:{(Application.Current.Resources["I18nConverter"] as I18nConverter)?
                                    .Convert("VALUE", typeof(string), null!, CultureInfo.InvariantCulture)}"
                            }
                        };
                        BindingProxy bp = (pre.ProvideValue((FindResource("SPC") as XamlServiceProviderCatcher)!.ServiceProvider!) as BindingProxy)!;
                        Binding binding = new("Value")
                        {
                            Source = bp
                        };
                        BindingOperations.SetBinding(column, DataGridTemplateColumn.HeaderTemplateProperty, binding);
                        DataGrid.Columns.Add(column);
                    }
                    else
                    {

                    }
                    column = new DataGridTemplateColumn();
                    column.CellTemplate = DataGrid.Resources["Actions"] as DataTemplate;
                    column.Header = (Application.Current.Resources["I18nConverter"] as I18nConverter)?
                        .Convert("ACTIONS", typeof(string), null!, CultureInfo.InvariantCulture);
                    DataGrid.Columns.Add(column);
                    _indexMapping.Clear();
                    int i = 0;
                    foreach (var item in (IList)_value)
                    {
                        _indexMapping.AddOrUpdate(item, i);
                        ++i;
                    }
                    ItemsDataGridManager.ViewSource.Source = _value;
                }
            }
        }
    }
    public EditList(string path, Type type, bool isReadonly = false)
    {
        IsReadonly = isReadonly;
        _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
        _context = _services.GetRequiredService<PocotaContext>();
        Windows = _services.GetRequiredService<WindowsList>();
        EditWindowCore = new EditWindowCore(path, type);
        InitializeComponent();
        Windows.Touch();
    }
    public bool CanExecute(object? parameter)
    {
        return parameter is PropertyAction action
            && (action is PropertyAction.Edit
            || (action is PropertyAction.Clear && !IsReadonly)
            || (action is PropertyAction.Create && !IsReadonly));
    }

    public void Execute(object? parameter)
    {
        if(parameter is PropertyAction action)
        {
            if (action is PropertyAction.Edit || action is PropertyAction.Create)
            {
                Property? property = null;
                if (action is PropertyAction.Edit)
                {

                }
                else if (action is PropertyAction.Create)
                {
                    if (!IsReadonly)
                    {
                        property = Property.Create(ItemType);
                    }
                }
                if (property is { })
                {
                    PropertyCommand cmd = new();
                    PropertyCommandArgs args = new()
                    {
                        Action = action,
                        Property = property,
                        Launcher = this
                    };
                    if (cmd.CanExecute(args))
                    {
                        cmd.Execute(args);
                    }
                }
            }
            else if (parameter is PropertyAction.Clear)
            {
                if (!IsReadonly && _indexMapping.TryGetValue(ItemsDataGridManager.ViewSource.View.CurrentItem, out object? index))
                {
                    _indexMapping.Remove(ItemsDataGridManager.ViewSource.View.CurrentItem);
                    ((IList)ItemsDataGridManager.ViewSource.View.SourceCollection).RemoveAt((int)index);
                    int i = 0;
                    foreach (var item in ItemsDataGridManager.ViewSource.View.SourceCollection)
                    {
                        _indexMapping.AddOrUpdate(item, i);
                        ++i;
                    }
                    ItemsDataGridManager.ViewSource.View.Refresh();
                }
            }
        }
    }
    internal object? GetIndex(object item)
    {
        if(_indexMapping.TryGetValue(item, out object? index))
        {
            return index;
        }
        return null;
    }
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (LaunchedBy is { })
        {
            LaunchedBy.Focus();
        }
    }
}
