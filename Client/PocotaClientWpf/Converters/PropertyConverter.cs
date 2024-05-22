using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class PropertyConverter : IValueConverter
{
    private StringBuilder _sb = new();
    private bool _invalidFormat = false;
    private object? _value = null;
    private bool _changed = false;
    public Property? Property {  get; set; }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine($"Convert: {value}, {Property?.Type} -> {targetType}, {parameter}");
        _value = value;
        if ("InvalidFormat".Equals(parameter))
        {
            return _invalidFormat;
        }
        else if ("RealValue".Equals(parameter))
        {
            return value is { }
                ? (string)System.Convert.ChangeType(value, typeof(string), GetFormatProvider(culture))
                : string.Empty;
        }
        else if (targetType == typeof(string) )
        {
            string valueString = value is { } 
                ? (string)System.Convert.ChangeType(value, typeof(string), GetFormatProvider(culture)) 
                : string.Empty;
            if (!_changed)
            {
                _sb.Clear();
                _sb.Append(valueString);
            }
            if(_sb.ToString() == valueString)
            {
                if (_invalidFormat)
                {
                    _invalidFormat = false;
                    Property?.NotifyPropertyChanged();
                }
            }
            else
            {
                if (!_invalidFormat)
                {
                    _invalidFormat = true;
                    Property?.NotifyPropertyChanged();
                }
            }
            _changed = false;
            return _sb.ToString();
        }
        return null;
    }
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

        Console.WriteLine($"ConvertBack: {value}, {targetType} -> {Property?.Type}, {parameter}, {culture}");
        IFormatProvider formatProvider = GetFormatProvider(culture);
        string valueString = value.ToString()!;
        _sb.Clear();
        _sb.Append(valueString);
        _changed = true;
        try
        {
            object? result = Property is null || (Property.IsNullable && string.IsNullOrEmpty(valueString)) 
                ? null 
                : System.Convert.ChangeType(valueString, Property.Type, formatProvider);
            return result;
        }
        catch
        {
            if (!_invalidFormat)
            {
                _invalidFormat = true;
                Property?.NotifyPropertyChanged();
            }
            return _value;
        }
    }

    private IFormatProvider GetFormatProvider(CultureInfo culture)
    {
        IFormatProvider? formatProvider = null!;
        if(Property is { })
        {
            if (typeof(decimal).IsAssignableFrom(Property.Type))
            {
                formatProvider = culture.NumberFormat;
            }
        }
        return formatProvider;
    }
}
