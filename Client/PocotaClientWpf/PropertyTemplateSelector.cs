using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Net.Leksi.Pocota.Client;
public class PropertyTemplateSelector: DataTemplateSelector
{
    public DataTemplate Default { get; set; } = null!;
    public DataTemplate Class { get; set; } = null!;
    public DataTemplate List { get; set; } = null!;
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        DataTemplate? result = null;
        Property? value = null;
        for (DependencyObject dob = container; dob is { }; dob = VisualTreeHelper.GetParent(dob))
        {
            if(dob is FrameworkElement fe && fe.DataContext is Property mp)
            {
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
                result = Default;
            }
            else if (value.Type.IsClass)
            {
                result = Class;
            }
            else
            {
                result = Default;
            }
        }
        return result;
    }
}
