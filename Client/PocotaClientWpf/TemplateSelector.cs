using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Net.Leksi.Pocota.Client;

public class TemplateSelector: DataTemplateSelector
{
    public DataTemplate HeaderDataTemplate { get; set; } = null!;
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        
        return HeaderDataTemplate;
    }
}
