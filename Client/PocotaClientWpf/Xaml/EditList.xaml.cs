using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;
public partial class EditList : Window, INotifyPropertyChanged, IEditWindow, ICommand, IWindowLauncher
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
    private readonly Trans _trans;
    private object? _value;
    private Window? _launchedBy;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private readonly IServiceProvider _windowXamlServices;
    private readonly IServiceProvider _dataGridXamlServices;
    public DataGridManager ItemsDataGridManager { get; private init; } = new();
    public WindowsList Windows { get; private init; }
    public bool IsReadonly { get; private init; }
    public bool IsDataGridReadonly => IsReadonly || ((ItemType?.IsClass ?? true) && ItemType != typeof(string));
    public Type ItemType { get; private set; } = null!;
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
    public EditWindowLauncher Launcher { get; private init; }
    public string? StringValue
    {
        get => ItemsDataGridManager.ViewSource.View.CurrentItem?.ToString();
        set
        {
            if (!IsReadonly && _indexMapping.TryGetValue(ItemsDataGridManager.ViewSource.View.CurrentItem, out object? index))
            {
                ((IList)ItemsDataGridManager.ViewSource.View.SourceCollection)[(int)index] = value;
            }
        }
    }
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
                    IList list;
                    DataGrid.Columns.Clear();
                    DataGridTemplateColumn column;
                    column = new DataGridTemplateColumn();
                    column.CellTemplate = DataGrid.Resources["Position"] as DataTemplate;
                    column.Header = _trans.Translate("POSITION");
                    DataGrid.Columns.Add(column);
                    if (!ItemType.IsClass || ItemType == typeof(string))
                    {
                        column = new DataGridTemplateColumn();
                        ParameterizedResourceExtension pre = new("Field")
                        {
                            Replaces = new string[] { "$field:Value", "$converter:EditListConverter" }
                        };
                        column.CellTemplate = pre.ProvideValue(_dataGridXamlServices) as DataTemplate;

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
                                $"$name:{_trans.Translate("VALUE")}"
                            }
                        };
                        BindingProxy bp = (pre.ProvideValue(_windowXamlServices) as BindingProxy)!;
                        Binding binding = new("Value")
                        {
                            Source = bp
                        };
                        BindingOperations.SetBinding(column, DataGridTemplateColumn.HeaderTemplateProperty, binding);
                        DataGrid.Columns.Add(column);
                        Type ItemHolderType = typeof(SimpleTypeHolder<>).MakeGenericType([ItemType]);
                        Type ListType = typeof(ObservableCollection<>).MakeGenericType(ItemHolderType);
                        list = (IList)Activator.CreateInstance(ListType)!;
                        int pos = 0;
                        PropertyInfo positionProperty = ItemHolderType.GetProperty(nameof(SimpleTypeHolder<object>.Position))!;
                        PropertyInfo valueProperty = ItemHolderType.GetProperty(nameof(SimpleTypeHolder<object>.Value))!;
                        foreach (object? item in (IList)_value)
                        {
                            object? holder = Activator.CreateInstance(ItemHolderType, _value);
                            positionProperty.SetValue(holder, pos);
                            list.Add(holder);
                            ++pos;
                        }
                    }
                    else
                    {
                        list = (IList)_value;
                    }
                    column = new DataGridTemplateColumn();
                    column.CellTemplate = DataGrid.Resources["Actions"] as DataTemplate;
                    column.Header = _trans.Translate("ACTIONS");
                    DataGrid.Columns.Add(column);
                    _indexMapping.Clear();
                    int i = 0;
                    foreach (var item in list)
                    {
                        _indexMapping.AddOrUpdate(item, i);
                        ++i;
                    }
                    ItemsDataGridManager.ViewSource.Source = list;
                    PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
                    DataGrid.IsReadOnly = IsDataGridReadonly;
                }
            }
        }
    }
    public EditList(string path, Type type, bool isReadonly = false)
    {
        IsReadonly = isReadonly;
        _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
        _context = _services.GetRequiredService<PocotaContext>();
        _trans = _services.GetRequiredService<Trans>();
        Windows = _services.GetRequiredService<WindowsList>();
        EditWindowCore = new EditWindowCore(path, type);
        Launcher = new EditWindowLauncher(EditWindowCore.Path, this);
        InitializeComponent();
        _windowXamlServices = (FindResource("WindowSP") as XamlServiceProviderCatcher)!.ServiceProvider!;
        _dataGridXamlServices = (DataGrid.FindResource("DataGridSP") as XamlServiceProviderCatcher)!.ServiceProvider!;
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
        if (parameter is PropertyAction action)
        {
            if (action is PropertyAction.Edit || action is PropertyAction.Create)
            {
                if (ItemType.IsClass && ItemType != typeof(string))
                {
                    Property? property = null;
                    if (action is PropertyAction.Edit)
                    {

                    }
                    else if (action is PropertyAction.Create)
                    {
                        if (!IsReadonly && _indexMapping.TryGetValue(ItemsDataGridManager.ViewSource.View.CurrentItem, out object? index))
                        {
                            property = Property.Create(new ParameterInfoCosplay($"<{_trans.Translate("NEW-ITEM")}>", ItemType));
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
                else
                {
                    ((IList)ItemsDataGridManager.ViewSource.View.SourceCollection).Add(ItemType != typeof(string) ? default : "");
                    int i = 0;
                    foreach (var item in ItemsDataGridManager.ViewSource.View.SourceCollection)
                    {
                        _indexMapping.AddOrUpdate(item, i);
                        ++i;
                    }
                    ItemsDataGridManager.ViewSource.View.Refresh();
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
        if (_indexMapping.TryGetValue(item, out object? index))
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
