using System.Globalization;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class ObjectToBooleanConverter : IValueConverter
{
    public ObjectToBooleanConverter()
    {
        Console.WriteLine("here");
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine(value);
        return value is null ? null : (bool)value;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine(value);
        return value is null ? null : (bool)value;
    }
}
