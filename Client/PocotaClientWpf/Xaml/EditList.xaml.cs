﻿using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;
public partial class EditList : Window, IEditWindow, ICommand
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
    private readonly Localizer _localizer;
    private Property? _property;
    private Window? _launchedBy;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private readonly IServiceProvider _windowXamlServices;
    private readonly IServiceProvider _dataGridXamlServices;
    private readonly PropertyCommand _propertyCommand = new();
    private PropertyCommandArgs? _editPropertyCommandArgs;
    private PropertyCommandArgs? _createPropertyCommandArgs;
    private PropertyCommandArgs? _clearPropertyCommandArgs;
    private string? _serviceKey = null;
    private PocotaContext? _context = null;
    public DataGridManager ItemsDataGridManager { get; private init; } = new();
    public WindowsList Windows { get; private init; }
    public bool IsReadonly { get; private init; }
    public bool KeysOnly { get; set; }
    public string ServiceKey
    {
        get => _serviceKey ?? string.Empty;
        set
        {
            if (_serviceKey is null && value != _serviceKey)
            {
                _serviceKey = value;
                _context = _services.GetRequiredKeyedService<PocotaContext>(ServiceKey);
            }
        }
    }
    public bool IsDataGridReadonly => IsReadonly || IsObject;
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
    public bool IsObject => (ItemType?.IsClass ?? false) && ItemType != typeof(string);
    public object? MovedItem { get; private set; } = null;
    public Property? Property
    {
        get => _property;
        set
        {
            if (_property != value && value is { })
            {
                if(_context is null)
                {
                    throw new InvalidOperationException("Must set ServiceKey first!");
                }
                if (
                    value.Type.IsGenericType
                    && value.Type.GetGenericArguments()[0] is Type itemType
                    && typeof(IList<>).MakeGenericType([itemType]).IsAssignableFrom(value.Type)
                )
                {
                    ItemType = itemType;
                    _createPropertyCommandArgs = new PropertyCommandArgs
                    {
                        Action = PropertyAction.Create,
                        Property = Property.Create(ItemType),
                        Launcher = this,
                        AltName = "[]",
                    };
                    _editPropertyCommandArgs = new PropertyCommandArgs
                    {
                        Action = PropertyAction.Edit,
                        Property = Property.Create(ItemType),
                        Launcher = this,
                        AltName = "[]",
                    };
                    _clearPropertyCommandArgs = new PropertyCommandArgs
                    {
                        Action = PropertyAction.Clear,
                        Property = Property.Create(ItemType),
                        Launcher = this,
                    };
                    ItemsDataGrid.IsReadOnly = IsDataGridReadonly;
                    _property = value;
                    IList list;
                    ItemsDataGrid.Columns.Clear();
                    DataGridTemplateColumn column;
                    column = new DataGridTemplateColumn
                    {
                        CellTemplate = ItemsDataGrid.Resources["Position"] as DataTemplate,
                        Header = _localizer.Position,
                        IsReadOnly = true
                    };
                    ItemsDataGrid.Columns.Add(column);
                    if (!IsObject)
                    {
                        ItemProperty = new SimpleListItemProperty((IList)_property.Value!, ItemType);
                        column = new DataGridTemplateColumn();
                        ParameterizedResourceExtension pre = new()
                        {
                            Replaces = new string[] { "$field:Value", "$converter:EditListConverter", "$serviceProviderCatcher:WindowSP" }
                        };
                        pre.ResourceKey = "Field";
                        column.CellTemplate = pre.ProvideValue(_dataGridXamlServices) as DataTemplate;
                        pre.ResourceKey = "PropertyTemplateSelector1";
                        PropertyTemplateSelector templateSelector = (pre.ProvideValue(_dataGridXamlServices) as PropertyTemplateSelector)!;
                        templateSelector.Replaces = ["$field:Value"];
                        column.CellEditingTemplateSelector = templateSelector;

                        DataGridConverter converter = new()
                        {
                            DataGridManager = ItemsDataGridManager,
                            FieldName = "Value"
                        };
                        ItemsDataGrid.Resources.Add("ValueConverter", converter);
                        pre = new("SortHeader1")
                        {
                            Replaces = new string[] {
                                "$converter:ValueConverter",
                                "$field:Value",
                                $"$name:{_localizer.Value}"
                            }
                        };
                        BindingProxy bp = (pre.ProvideValue(_dataGridXamlServices) as BindingProxy)!;
                        Binding binding = new("Value")
                        {
                            Source = bp
                        };
                        BindingOperations.SetBinding(column, DataGridTemplateColumn.HeaderTemplateProperty, binding);

                        ItemsDataGrid.Columns.Add(column);
                        list = new ObservableCollection<SimpleListItemProperty>();
                        int pos = 0;
                        foreach (object? item in (IList)_property.Value!)
                        {
                            list.Add(new SimpleListItemProperty((IList)_property.Value, itemType));
                            ++pos;
                        }
                    }
                    else
                    {
                        list = (IList)_property.Value!;
                        ItemProperty = null;
                        if (typeof(IEntityOwner).IsAssignableFrom(ItemType))
                        {
                            IPocotaEntity probe = (list.Count == 0 ? _context!.CreateEntity(ItemType)?.Entity : ((IEntityOwner)list[0]!).Entity)!;
                            foreach(EntityProperty prop in probe.Properties)
                            {
                                ItemProperty = Property.Create(prop);
                                EditListConverter editListConverter = new()
                                {
                                    Owner = this
                                };
                                ItemsDataGrid.Resources[$"{prop.Name}EditListConverter"] = editListConverter;
                                column = new DataGridTemplateColumn();
                                ParameterizedResourceExtension pre = new()
                                {
                                    Replaces = new string[] { $"$field:{prop.Name}", $"$converter:{prop.Name}EditListConverter" }
                                };
                                pre.ResourceKey = ItemProperty is ListProperty ? "Collection" : "Field";
                                column.CellTemplate = pre.ProvideValue(_dataGridXamlServices) as DataTemplate;
                                ItemsDataGrid.Resources.Remove($"{prop.Name}EditListConverter");

                                DataGridConverter converter = new()
                                {
                                    DataGridManager = ItemsDataGridManager,
                                    FieldName = prop.Name
                                };
                                ItemsDataGrid.Resources.Add($"{prop.Name}ValueConverter", converter);
                                pre = new("SortHeader1")
                                {
                                    Replaces = new string[] {
                                        $"$converter:{prop.Name}ValueConverter",
                                        $"$field:{prop.Name}",
                                        $"$name:{prop.Name}"
                                    }
                                };
                                BindingProxy bp = (pre.ProvideValue(_dataGridXamlServices) as BindingProxy)!;
                                Binding binding = new("Value")
                                {
                                    Source = bp
                                };
                                BindingOperations.SetBinding(column, DataGridTemplateColumn.HeaderTemplateProperty, binding);
                                ItemsDataGrid.Resources.Remove($"{prop.Name}ValueConverter");

                                ItemsDataGrid.Columns.Add(column);
                                ItemProperty = null;
                            }
                        }
                        else
                        {

                        }
                    }
                    column = new DataGridTemplateColumn
                    {
                        CellTemplate = ItemsDataGrid.Resources["Actions"] as DataTemplate,
                        Header = _localizer.Actions,
                        IsReadOnly = true
                    };
                    ItemsDataGrid.Columns.Add(column);
                    _indexMapping.Clear();
                    ItemsDataGridManager.ViewSource.Source = list;
                    ItemsDataGridManager.ViewSource.View.CurrentChanged += View_CurrentChanged;
                    RenumberItems();
                    PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
                }
            }
        }
    }
    public Property? ItemProperty { get; private set; } = null;
    public EditList(string path, Type type, bool isReadonly = false)
    {
        IsReadonly = isReadonly;
        _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
        _localizer = _services.GetRequiredService<Localizer>();
        Windows = _services.GetRequiredService<WindowsList>();
        EditWindowCore = new EditWindowCore(path, type);
        Launcher = new EditWindowLauncher(EditWindowCore.Path, this);
        InitializeComponent();
        ItemsDataGridManager.DataGrid = ItemsDataGrid;
        ItemsDataGridManager.AutoCommit = true;
        _windowXamlServices = (FindResource("WindowSP") as XamlServiceProviderCatcher)!.ServiceProvider!;
        _dataGridXamlServices = (ItemsDataGrid.FindResource("DataGridSP") as XamlServiceProviderCatcher)!.ServiceProvider!;
        Windows.Touch();
    }
    protected override void OnClosed(EventArgs e)
    {
        Windows.Touch();
        _launchedBy?.Activate();
        base.OnClosed(e);
    }
    private void View_CurrentChanged(object? sender, EventArgs e)
    {
        PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
    }
    public bool CanExecute(object? parameter)
    {
        return 
            parameter is EditListCommandArgs args
            && (
                (
                    args.Action is PropertyAction.Edit 
                    && IsObject 
                    && (_editPropertyCommandArgs!.Property!.Value = args.Item) is { }
                    && _propertyCommand.CanExecute(_editPropertyCommandArgs)
                )
                || (
                    args.Action is PropertyAction.Clear 
                    && !IsReadonly 
                    && (_clearPropertyCommandArgs!.Property!.Value = args.Item) is { }
                    && _propertyCommand.CanExecute(_clearPropertyCommandArgs)
                )
                || (
                    args.Action is PropertyAction.Create 
                    && !IsReadonly
                    && (
                        _createPropertyCommandArgs is null
                        || (_createPropertyCommandArgs!.Property!.Value = null) is null
                    )
                    && _propertyCommand.CanExecute(_createPropertyCommandArgs)
                )
                || (
                    args.Action is PropertyAction.InsertBefore 
                    && !IsReadonly 
                    && args.Item is { }
                    && _propertyCommand.CanExecute(_createPropertyCommandArgs)
                )
                || (
                    args.Action is PropertyAction.Move 
                    && !IsReadonly 
                    && (
                        args.Item is { } 
                        || MovedItem is { }
                    ) 
                    && (((IList?)_property?.Value)?.Count ?? 0) > 1
                )
            );
    }
    public void Execute(object? parameter)
    {
        if (parameter is EditListCommandArgs args)
        {
            if (
                (args.Action is PropertyAction.Edit && IsObject && args.Item is { }) 
                || (args.Action is PropertyAction.Create && !IsReadonly) 
                || (args.Action is PropertyAction.InsertBefore && !IsReadonly && args.Item is { })
            )
            {
                if (args.Action is PropertyAction.Edit)
                {
                    if (IsObject)
                    {
                        _editPropertyCommandArgs!.Property!.Value = args.Item;
                        if (_propertyCommand.CanExecute(_editPropertyCommandArgs))
                        {
                            _propertyCommand.Execute(_editPropertyCommandArgs);
                        }
                    }
                }
                else if (!IsReadonly)
                {
                    int insertPos = args.Action is PropertyAction.InsertBefore
                        && args.Item is { }
                        && _indexMapping.TryGetValue(args.Item, out object? index)
                        ? (int)index! : ((IList)_property!.Value!).Count;
                    object? item = null;
                    if (IsObject)
                    {
                        if (_propertyCommand.CanExecute(_createPropertyCommandArgs))
                        {
                            _propertyCommand.Execute(_createPropertyCommandArgs);
                            item = _createPropertyCommandArgs!.Property!.Value;
                        }
                    }
                    else
                    {
                        ((IList)_property!.Value!).Insert(insertPos, ItemType == typeof(string) ? null : Activator.CreateInstance(ItemType));
                        item = new SimpleListItemProperty((IList)_property.Value, ItemType);
                    }
                    if (item is { })
                    {
                        ((IList)ItemsDataGridManager.ViewSource.View.SourceCollection).Insert(insertPos, item);
                        ItemsDataGrid.CommitEdit();
                        RenumberItems();
                        ItemsDataGridManager.ViewSource.View.Refresh();
                        ItemsDataGridManager.ViewSource.View.MoveCurrentTo(item);
                    }
                }
            }
            else if (args.Action is PropertyAction.Clear)
            {
                if (!IsReadonly && args.Item is { } && _indexMapping.TryGetValue(args.Item, out object? index))
                {
                    _indexMapping.Remove(args.Item);
                    ((IList)ItemsDataGridManager.ViewSource.View.SourceCollection).RemoveAt((int)index);
                    if (!IsObject)
                    {
                        ((IList)_property!.Value!).RemoveAt((int)index);
                    }
                    RenumberItems();
                    ItemsDataGridManager.ViewSource.View.Refresh();
                }
            }
            else if(args.Action is PropertyAction.Move)
            {
                if(!IsReadonly && (args.Item is { } || MovedItem is { }) && ((IList)_property!.Value!).Count > 1) {
                    if(MovedItem is null)
                    {
                        MovedItem = args.Item;
                    }
                    else if(MovedItem == args.Item)
                    {
                        MovedItem = null;
                    }
                    else
                    {
                        if (!IsReadonly && _indexMapping.TryGetValue(MovedItem, out object? index))
                        {
                            object? savedValue = IsObject ? MovedItem : ((SimpleListItemProperty)MovedItem).Value;
                            _indexMapping.Remove(MovedItem);
                            ((IList)ItemsDataGridManager.ViewSource.View.SourceCollection).RemoveAt((int)index);
                            if (!IsObject)
                            {
                                ((IList)_property!.Value!).RemoveAt((int)index);
                            }
                            RenumberItems();
                            int insertPos = args.Item is { } && _indexMapping.TryGetValue(args.Item, out object? index1)
                                ? (int)index1! : ((IList)_property!.Value!).Count;
                            if (!IsObject)
                            {
                                ((IList)_property!.Value!).Insert(insertPos, savedValue);
                            }
                            ((IList)ItemsDataGridManager.ViewSource.View.SourceCollection).Insert(insertPos, MovedItem);

                            RenumberItems();
                            ItemsDataGridManager.ViewSource.View.Refresh();
                        }
                        MovedItem = null;
                    }
                }
            }
            PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            _property?.NotifyPropertyChanged();
        }
    }
    internal object? GetIndex(object item)
    {
        if (item is { } && _indexMapping.TryGetValue(item, out object? index))
        {
            return index;
        }
        return null;
    }
    private void RenumberItems()
    {
        int i = 0;
        foreach (var item in ItemsDataGridManager.ViewSource.View.SourceCollection)
        {
            _indexMapping.AddOrUpdate(item, i);
            if (!IsObject)
            {
                ((SimpleListItemProperty)item).Position = i;
            }
            ++i;
        }
    }
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (LaunchedBy is { })
        {
            LaunchedBy.Focus();
        }
    }
    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ItemsDataGrid.BeginEdit();
    }
}
