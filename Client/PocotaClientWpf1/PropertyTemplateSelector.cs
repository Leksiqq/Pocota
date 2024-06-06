using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xaml;

namespace Net.Leksi.Pocota.Client;

public class PropertyTemplateSelector: DataTemplateSelector
{
    public XamlServiceProviderCatcher? ServiceProviderCatcher { get; set; }
    public string ClassDataTemplateKey { get; set; } = null!;
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        DataTemplate? result = null;
        if (item is Property property)
        {
            if (property is ListProperty)
            {
            }
            else if (property.Type.IsClass && property.Type != typeof(string))
            {
                result = ProvideValue(ClassDataTemplateKey);
            }
            else if (
                (property.Type.IsEnum || property.Type == typeof(bool))
                || (
                    property.Type.IsGenericType
                    && property.Type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && (property.Type.GetGenericArguments()[0].IsEnum || property.Type.GetGenericArguments()[0] == typeof(bool))
                )
            )
            {
            }
            else
            {
            }
            if(result is { })
            {
                return result;
            }
        }
        return base.SelectTemplate(item, container);
    }
    private DataTemplate? ProvideValue(string templateKey)
    {
        Console.WriteLine($"{GetType()}: ProvideValue");
        DataTemplate? result = null;
        if (ServiceProviderCatcher is { })
        {
            IServiceProvider sp = ServiceProviderCatcher.ServiceProvider!;
            ParameterizedResourceExtension pre = new(templateKey);
            result = pre.ProvideValue(sp) as DataTemplate;
        }
        return result;
    }
}
