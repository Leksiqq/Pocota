using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class EnumField : UserControl, IValueConverter
{
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
       nameof(Property), typeof(Property),
       typeof(EnumField)
    );
    public Property Property
    {
        get => (Property)GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }
    public EnumField()
    {
        InitializeComponent();
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("Enum".Equals(parameter))
        {
            List<string> result = [];
            if(Property is { })
            {
                Type enumType = Property.Type;
                if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
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
            }
            return result;
        }
        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == PropertyProperty)
        {
            if (e.OldValue is Property oldProperty)
            {
                ComboBox.DataContext = null;
            }
            if (e.NewValue is Property newProperty)
            {
                ComboBox.DataContext = newProperty;
            }
        }
        base.OnPropertyChanged(e);
    }
}
