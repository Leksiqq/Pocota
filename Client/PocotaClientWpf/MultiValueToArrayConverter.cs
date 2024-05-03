using System.Globalization;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class MultiValueToArrayConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.ToArray();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [.. ((object[])value)];
    }
}
