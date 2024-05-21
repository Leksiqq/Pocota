using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Net.Leksi.Pocota.Client;
public class PropertyTemplateSelector: DataTemplateSelector
{
    public object DefaultKey { get; set; } = null!;
    public DataTemplate Class { get; set; } = null!;
    public DataTemplate List { get; set; } = null!;
    private DataTemplate? Default = null;
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        DataTemplate? result = null;
        Property? value = null;
        for (DependencyObject dob = container; dob is { }; dob = VisualTreeHelper.GetParent(dob))
        {
            if(dob is FrameworkElement fe && fe.DataContext is Property mp)
            {
                Default = fe.FindResource(DefaultKey) as DataTemplate;
                value = mp;
                break;
            }
        }
        if (value is { })
        {
            if(value is ListProperty)
            {
                result = List;
            }
            else if(value.Type == typeof(string))
            {
                Console.WriteLine($"{value.Name}, {Default.GetHashCode()}");
                result = Default;
            }
            else if (value.Type.IsClass)
            {
                result = Class;
            }
            else
            {
                Console.WriteLine($"{value.Name}, {Default.GetHashCode()}");
                result = Default;
            }
        }
        return result;
    }
}
