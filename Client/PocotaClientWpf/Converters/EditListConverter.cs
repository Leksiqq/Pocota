using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace Net.Leksi.Pocota.Client;
public class EditListConverter : Freezable, IValueConverter
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
        Console.WriteLine($"Convert: {value}, {parameter}, {targetType}");
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
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine($"ConvertBack: {value}, {parameter}, {targetType}");
        return value;
    }

    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}
