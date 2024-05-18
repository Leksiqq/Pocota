using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace Net.Leksi.Pocota.Client;
public class EditListConverter : Freezable, IValueConverter,IMultiValueConverter
{
    public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register(
       nameof(Owner), typeof(EditList),
       typeof(EditListConverter)
    );
    public EditList? Owner
    {
        get => (EditList)GetValue(OwnerProperty);
        set => SetValue(OwnerProperty, value);
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(parameter.ToString() == "Position")
        {
            if (Owner?.GetIndex(value) is int index)
            {
                return index;
            }
            return null;
        }
        if (parameter.ToString() == "Edit")
        {
            return value;
        }
        Console.WriteLine($"Convert: {value}, {parameter}, {targetType}");
        return value;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter.ToString() == "IsMovedItemSource")
        {
            return values.Length > 0 && Owner!.MovedItem is { } && Owner.MovedItem == values[0];
        }
        if (parameter.ToString() == "IsMovedItemTarget")
        {
            return values.Length > 0 && Owner!.MovedItem is { } && Owner.MovedItem != values[0];
        }
        Console.WriteLine($"Convert multi: {string.Join(", ", values)}, {parameter}, {targetType}");
        return values.ToArray();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
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
