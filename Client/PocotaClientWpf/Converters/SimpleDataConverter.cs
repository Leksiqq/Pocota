using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class SimpleDataConverter : Freezable, IValueConverter, IMultiValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine($"Convert: {value}, {targetType}, {parameter}");
        return value;
    }

    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine($"Convert: {string.Join(',', values)}, {targetType}, {parameter}");
        return values.FirstOrDefault();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine($"ConvertBack: {value}, {targetType}, {parameter}");
        return value;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        Console.WriteLine($"ConvertBack: {value}, {string.Join(',', targetTypes.Select(t => t.Name))}, {parameter}");
        return [value];
    }

    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}
