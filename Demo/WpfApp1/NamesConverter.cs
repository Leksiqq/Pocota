using Net.Leksi.Pocota.Client;
using Net.Leksi.WpfMarkup;
using System.Globalization;

namespace WpfApp1;

public class NamesConverter : INamesConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine($"{value}, {parameter}");
        return value?.ToString()!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
