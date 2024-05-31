using System.Globalization;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class IsNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if(parameter is { })
        {
            Console.WriteLine($"{GetType()}.Convert: {parameter}");
        }
        return value is null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
