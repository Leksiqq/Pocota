using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace Net.Leksi.Pocota.Client;
public class EditListConverter : Freezable, IValueConverter
{
    public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register(
       nameof(Owner), typeof(EditList),
       typeof(EditListConverter)
    );
    public EditList? Owner
    {
        get => (EditList)GetValue(OwnerProperty);
        set => SetValue(OwnerProperty, value);
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Owner?.GetIndex(value) is int index)
        {
            return index;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}
