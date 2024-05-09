using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.XPath;

namespace Net.Leksi.Pocota.Client;

public class MethodParameterTemplateSelector: DataTemplateSelector
{
    public DataTemplate Default { get; set; } = null!;
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        DataTemplate result = Default;
        return result;
    }
}
