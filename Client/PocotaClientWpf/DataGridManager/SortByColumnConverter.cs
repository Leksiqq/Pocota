using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class SortByColumnConverter: Freezable, IValueConverter
{
    public static readonly DependencyProperty FieldNameProperty = DependencyProperty.Register(
       "FieldName", typeof(string),
       typeof(SortByColumnConverter)
    );
    public static readonly DependencyProperty DataGridManagerProperty = DependencyProperty.Register(
        "DataGridManager", typeof(DataGridManager),
        typeof(SortByColumnConverter)
    );
    public string? FieldName
    {
        get => (string)GetValue(FieldNameProperty);
        set => SetValue(FieldNameProperty, value);
    }
    public DataGridManager? DataGridManager
    {
        get => (DataGridManager)GetValue(DataGridManagerProperty);
        set => SetValue(DataGridManagerProperty, value);
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DataGridManager?.Convert(FieldName, value, targetType, parameter, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //Console.WriteLine($"ConvertBack {GetHashCode()}: {value}, {FieldName}, {parameter}");
        return value;
    }

    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}
