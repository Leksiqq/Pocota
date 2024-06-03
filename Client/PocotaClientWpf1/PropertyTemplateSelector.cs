using System.Windows;
using System.Windows.Controls;

namespace Net.Leksi.Pocota.Client;

public class PropertyTemplateSelector: DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return base.SelectTemplate(item, container);
    }
}
