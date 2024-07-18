using System.Globalization;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class LocalizeConverter : Localizer, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return GetString(parameter.ToString()!);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
