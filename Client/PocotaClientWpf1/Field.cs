using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
namespace Net.Leksi.Pocota.Client;
public class Field
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(nameof(Value));
    private readonly NullabilityInfoContext _nullability = new();
    private IFieldOwner? _owner;
    private object? _target;
    private string? _propertyName;
    private EntityProperty? _entityProperty;
    private PropertyInfo? _propertyInfo;
    private bool _isNullable = false;
    private bool _isCollection = false;
    private Type _type = null!;
    private bool _assignedFieldCalled = false;
    public object? Target
    {
        get => _target;
        set 
        {
            if(_target is null && value is { })
            {
                _target = value;
                if(_target is INotifyPropertyChanged npc)
                {
                    WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(npc, nameof(PropertyChanged), Npc_PropertyChanged);
                }
                ProcessPropertyChanged();
            }
        }
    }
    public string? PropertyName
    {
        get => _propertyName;
        set
        {
            if (_propertyName != value)
            {
                _propertyName = value;
                ProcessPropertyChanged();
            }
        }
    }
    public IFieldOwner? Owner
    {
        get => _owner;
        set
        {
            if (_owner is null && value is { })
            {
                _owner = value;
                _owner.Field = this;
                if (IsReady && !_assignedFieldCalled)
                {
                    _assignedFieldCalled = true;
                    _owner.OnFieldAssigned();
                }
            }
        }
    }
    public Type? Declarator => _target?.GetType();
    public EntityProperty? EntityProperty => _entityProperty;
    public Type Type
    {
        get
        {
            if(_type is null)
            {
                throw new InvalidOperationException("Field is not ready!");
            }
            return  _type;
        }
    }
    public bool IsNullable => _isNullable;
    public bool IsCollection => _isCollection;
    public int Count => IsReady && IsCollection ? (Value as IList)?.Count ?? 0 : 0;
    public bool IsReadonly  => _entityProperty?.IsReadonly ?? !(_propertyInfo?.CanWrite ?? false);
    public object? Value
    {
        get => _propertyInfo?.GetValue(Target);
        set
        {
            if(_propertyInfo is { })
            {
                _propertyInfo?.SetValue(Target, value);
            }
        }
    }
    public bool IsClean
    {
        get
        {
            if (Type is { })
            {
                if (IsNullable || Type.IsClass)
                {
                    return Value is null;
                }
                return Value?.Equals(Activator.CreateInstance(Type)) ?? true;
            }
            return true;
        }
    }
    public bool IsReady => _type is { };
    public void Clear()
    {
        if (Type is { })
        {
            if (IsNullable || Type.IsClass)
            {
                Value = null;
            }
            else
            {
                Value = Activator.CreateInstance(Type);
            }
        }
    }
    private void Npc_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(sender == _target && e.PropertyName == PropertyName)
        {
            PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
        }
    }
    private void ProcessPropertyChanged()
    {
        if(Target is { } && PropertyName is { })
        {
            _propertyInfo = Target.GetType().GetProperty(PropertyName);
            if (_propertyInfo is null)
            {
                throw new ArgumentException($"{Target.GetType()} has not {nameof(PropertyName)} property!");
            }
            _entityProperty = Target is IEntityOwner eo ? eo.Entity.GetEntityProperty(PropertyName) : null;
            _type = _entityProperty is { } ? _entityProperty.Type : _propertyInfo.PropertyType;
            _isNullable = false;
            if(_type.IsGenericType && typeof(Nullable<>).IsAssignableFrom(_type.GetGenericTypeDefinition()))
            {
                _isNullable = true;
                _type = _type.GetGenericArguments()[0];
            }
            else if(_nullability.Create(_propertyInfo!).ReadState is NullabilityState.Nullable)
            {
                _isNullable = true;
            }
            _isCollection = _type.IsGenericType && typeof(ObservableCollection<>).IsAssignableFrom(_type.GetGenericTypeDefinition());
            if (_owner is { } && !_assignedFieldCalled)
            {
                _assignedFieldCalled = true;
                _owner.OnFieldAssigned();
            }
        }
    }
}
