using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Net.Leksi.Pocota.Client;

public class TypeNameConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is Type type)
        {
            if ("Short".Equals(parameter))
            {
                return Util.BuildTypeName(type);
            }
            if ("Full".Equals(parameter))
            {
                return Util.BuildTypeFullName(type);
            }
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
