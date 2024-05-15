using Net.Leksi.WpfMarkup;
using System.Globalization;
using System.Windows;

namespace Net.Leksi.Pocota.Client;

public class DataGridConverter : SortByColumnConverter
{
    public static string SortPositionText { get; private set; } = "sortPositionText";
    public static string SortPositionVisibility { get; private set; } = "sortPositionVisibility";

    public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter.Equals(SortPositionText))
        {
            int pos = (int)base.Convert(value, targetType, SortByColumnConverter.SortPosition, culture)!;
            if (pos >= 0)
            {
                return (pos + 1).ToString();
            }
            return string.Empty;
        }
        if (parameter.Equals(SortPositionVisibility))
        {
            int pos = (int)base.Convert(value, targetType, SortByColumnConverter.SortPosition, culture)!;
            return pos >= 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        return base.Convert(value, targetType, parameter, culture);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
