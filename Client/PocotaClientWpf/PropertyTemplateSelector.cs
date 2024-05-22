using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Xaml;

namespace Net.Leksi.Pocota.Client;
public class PropertyTemplateSelector: DataTemplateSelector
{
    public string DefaultDataTemplateKey { get; set; } = null!;
    public XamlServiceProviderCatcher ServiceProviderCatcher { get; set; } = null!;
    public DataTemplate ClassDataTemplate { get; set; } = null!;
    public DataTemplate ListDataTemplate { get; set; } = null!;

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        DataTemplate? result = null;
        Property? value = null;
        FrameworkElement? frameworkElement = null;
        for (DependencyObject dob = container; dob is { }; dob = VisualTreeHelper.GetParent(dob))
        {
            if(dob is FrameworkElement fe && fe.DataContext is Property mp)
            {
                frameworkElement = fe;
                value = mp;
                break;
            }
        }
        if (value is { })
        {
            if(value is ListProperty)
            {
                result = ListDataTemplate;
            }
            else if (value.Type.IsClass && value.Type != typeof(string))
            {
                result = ClassDataTemplate;
            }
            else
            {
                IServiceProvider sp = ServiceProviderCatcher.ServiceProvider!;
                IRootObjectProvider rop = sp.GetRequiredService<IRootObjectProvider>();
                string converterKey = $"{value.Name}Converter{Guid.NewGuid()}";
                (rop.RootObject as Window)!.Resources[converterKey] = new PropertyConverter { Property = value };
                ParameterizedResourceExtension pre = new(DefaultDataTemplateKey);
                pre.Replaces = new string[] { $"$converter:{converterKey}" };
                result = pre.ProvideValue(sp) as DataTemplate;
                (rop.RootObject as Window)!.Resources.Remove(converterKey);
            }
        }
        return result;
    }
}
