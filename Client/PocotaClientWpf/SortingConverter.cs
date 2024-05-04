using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class SortingConverter: Freezable, IValueConverter
{
    public static readonly DependencyProperty CollectionViewSourceProperty = DependencyProperty.Register(
        "CollectionViewSource", typeof(CollectionViewSource),
        typeof(SortingConverter)
    );
    public string? FieldName {  get; set; }
    public CollectionViewSource? CollectionViewSource
    {
        get => (CollectionViewSource)GetValue(CollectionViewSourceProperty);
        set => SetValue(CollectionViewSourceProperty, value);
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(parameter.ToString() == "sortPositionVisibility")
        {
            if (
                CollectionViewSource is { }
                && Enumerable.Range(0, CollectionViewSource!.SortDescriptions.Count).Where(i => CollectionViewSource.SortDescriptions[i].PropertyName == FieldName)
                    .FirstOrDefault(-1) is int pos 
                && pos >= 0
            )
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        else if (parameter.ToString() == "sortPositionText")
        {
            if (
                CollectionViewSource is { }
                && Enumerable.Range(0, CollectionViewSource!.SortDescriptions.Count).Where(i => CollectionViewSource.SortDescriptions[i].PropertyName == FieldName)
                    .FirstOrDefault(-1) is int pos
                && pos >= 0
            )
            {
                return (pos + 1).ToString();
            }
            return string.Empty;
        }
        else if(parameter.ToString() == "tag")
        {
            return (bool?)value;
        }
        else
        {
            Console.WriteLine($"Convert {GetHashCode()}: {value}, {FieldName}, {parameter}");
        }
        return value;
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
