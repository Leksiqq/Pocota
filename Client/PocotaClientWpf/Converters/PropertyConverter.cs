using System.Globalization;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class PropertyConverter : IValueConverter
{
    private object? _value;
    private bool _invalidFormat = false;
    public Property? Property {  get; set; }
    public object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        Console.WriteLine($"{GetType()}.Convert: {value}, {Property?.Type} -> {targetType}, {parameter}");
        if ("InvalidFormat".Equals(parameter))
        {
            return _invalidFormat;
        }
        if ("IsReadonly".Equals(parameter))
        {
            return value;
        }
        if ("Enum".Equals(parameter))
        {
            Type enumType = Property!.Type;
            List<string> result = [];
            if(enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                enumType = enumType.GetGenericArguments()[0];
                result.Add(string.Empty);
            }
            if (enumType.IsEnum)
            {
                result.AddRange(Enum.GetNames(enumType));
            }
            else
            {
                result.Add(true.ToString());
                result.Add(false.ToString());
            }
            return result;
        }
        if (_invalidFormat)
        {
            return _value;
        }
        return value is { } && Property is { }
            ? InternalConvert(value, GetFormatProvider(culture))
            : string.Empty;
    }
    public object? ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        Console.WriteLine($"ConvertBack: {value}, {targetType} -> {Property?.Type}, {parameter}, {culture}");
        _value = value;
        try
        {
            string? valueString = value?.ToString();
            object? result = Property is null || (Property.IsNullable && string.IsNullOrEmpty(valueString))
                ? null
                : InternalConvertBack(valueString!, GetFormatProvider(culture));
            if (_invalidFormat)
            {
                _invalidFormat = false;
                Property?.NotifyPropertyChanged();
            }
            return result;
        }
        catch
        {
            if (!_invalidFormat)
            {
                _invalidFormat = true;
                Property?.NotifyPropertyChanged();
            }
            return null;
        }
    }
    private string InternalConvert(object value, IFormatProvider formatProvider)
    {
        string? result;
        if (
                typeof(DateOnly).IsAssignableFrom(Property!.Type)
                || typeof(DateOnly?).IsAssignableFrom(Property.Type)
            )
        {
            result = ((DateOnly)value).ToString(formatProvider);
        }
        else if (
                typeof(TimeOnly).IsAssignableFrom(Property!.Type)
                || typeof(TimeOnly?).IsAssignableFrom(Property.Type)
            )
        {
            result = ((TimeOnly)value).ToString(((DateTimeFormatInfo)formatProvider).LongTimePattern, formatProvider);
        }
        else if (Property is ListProperty)
        {
            result = "list";
        }
        else
        {
            result = (string)System.Convert.ChangeType(value, typeof(string), formatProvider);
        }

        return result;
    }
    private object? InternalConvertBack(string valueString, IFormatProvider formatProvider)
    {
        object? result;
        if (
            typeof(DateTime).IsAssignableFrom(Property!.Type)
            || typeof(DateTime?).IsAssignableFrom(Property.Type)
        )
        {
            result = DateTime.Parse(valueString, formatProvider);
        }
        else if (
            typeof(DateOnly).IsAssignableFrom(Property!.Type)
            || typeof(DateOnly?).IsAssignableFrom(Property.Type)
        )
        {
            result = DateOnly.Parse(valueString, formatProvider);
        }
        else if (
            typeof(TimeOnly).IsAssignableFrom(Property!.Type)
            || typeof(TimeOnly?).IsAssignableFrom(Property.Type)
        )
        {
            result = TimeOnly.Parse(valueString, formatProvider);
        }
        else
        {
            Type type = Property!.Type.IsGenericType && Property!.Type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Property!.Type.GetGenericArguments()[0]
                    : Property!.Type;
            if (type.IsEnum)
            {
                result = Enum.Parse(type, valueString);
            }
            else
            {
                result = System.Convert.ChangeType(
                    valueString,
                    type,
                    formatProvider
                );
            }
        }
        Console.WriteLine($"{valueString} as {Property!.Type} => {result}");
        return result;
    }
    private IFormatProvider GetFormatProvider(CultureInfo culture)
    {
        IFormatProvider? formatProvider = null!;
        if (Property is { })
        {
            if (typeof(decimal).IsAssignableFrom(Property.Type))
            {
                formatProvider = culture.NumberFormat;
            }
            else if (
                typeof(DateTime).IsAssignableFrom(Property.Type)
                || typeof(DateTime?).IsAssignableFrom(Property.Type)
                || typeof(DateOnly).IsAssignableFrom(Property.Type)
                || typeof(DateOnly?).IsAssignableFrom(Property.Type)
                || typeof(TimeOnly).IsAssignableFrom(Property.Type)
                || typeof(TimeOnly?).IsAssignableFrom(Property.Type)
            )
            {
                formatProvider = culture.DateTimeFormat;
            }
        }
        return formatProvider;
    }
}
