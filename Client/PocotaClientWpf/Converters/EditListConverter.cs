using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace Net.Leksi.Pocota.Client;
public class EditListConverter : Freezable, IValueConverter, IMultiValueConverter
{
    public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register(
       nameof(Owner), typeof(EditList),
       typeof(EditListConverter)
    );
    private PropertyConverter _propertyConverter = new();
    public EditList? Owner
    {
        get => (EditList)GetValue(OwnerProperty);
        set => SetValue(OwnerProperty, value);

        //{
        //    if (value != Owner)
        //    {
        //        if(Owner is { })
        //        {
        //            _propertyConverter = null;
        //            Owner.PropertyChanged -= Owner_PropertyChanged;
        //        }
        //        if (Owner is { })
        //        {
        //            Owner.PropertyChanged += Owner_PropertyChanged;
        //        }
        //    }
        //}
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if(e.Property == OwnerProperty)
        {
            if (e.OldValue is EditList el)
            {
                el.PropertyChanged -= Owner_PropertyChanged;
            }
            if (e.NewValue is EditList el1)
            {
                el1.PropertyChanged += Owner_PropertyChanged;
                if (Owner?.CurrentProperty is { })
                {
                    _propertyConverter.Property = Owner.CurrentProperty;
                }
            }
        }
        base.OnPropertyChanged(e);
    }

    private void Owner_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(Owner?.CurrentProperty is { })
        {
            _propertyConverter.Property = Owner.CurrentProperty;
        }
    }

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if("Position".Equals(parameter))
        {
            if (Owner?.GetIndex(value) is int index)
            {
                return index;
            }
            return null;
        }
        if ("Edit".Equals(parameter))
        {
            return _propertyConverter.Convert(value, targetType, parameter, culture);
        }
        Console.WriteLine($"Convert: {value}, {parameter}, {targetType}");
        return value;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if ("IsMovedItemSource".Equals(parameter))
        {
            return values.Length > 0 && Owner!.MovedItem is { } && Owner.MovedItem == values[0];
        }
        if ("IsMovedItemTarget".Equals(parameter))
        {
            return values.Length > 0 && Owner!.MovedItem is { } && Owner.MovedItem != values[0];
        }
        Console.WriteLine($"Convert multi: {string.Join(", ", values)}, {parameter}, {targetType}");
        return values.ToArray();
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("Edit".Equals(parameter))
        {
            return _propertyConverter?.ConvertBack(value, targetType, parameter, culture);
        }
        Console.WriteLine($"ConvertBack: {value}, {parameter}, {targetType}");
        return value;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}
