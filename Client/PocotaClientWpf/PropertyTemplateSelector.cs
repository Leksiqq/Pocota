using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xaml;

namespace Net.Leksi.Pocota.Client;
public class PropertyTemplateSelector: DataTemplateSelector
{
    public XamlServiceProviderCatcher ServiceProviderCatcher { get; set; } = null!;
    public string DefaultDataTemplateKey { get; set; } = null!;
    public string EnumDataTemplateKey { get; set; } = null!;
    public string ClassDataTemplateKey { get; set; } = null!;
    public string ListDataTemplateKey { get; set; } = null!;
    public string[]? Replaces { get; set; } = null;

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
                result = ProvideValue(value, ListDataTemplateKey);
            }
            else if (value.Type.IsClass && value.Type != typeof(string))
            {
                result = ProvideValue(value, ClassDataTemplateKey);
            }
            else if (
                (value.Type.IsEnum || value.Type == typeof(bool))
                || (
                    value.Type.IsGenericType 
                    && value.Type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && (value.Type.GetGenericArguments()[0].IsEnum || value.Type.GetGenericArguments()[0] == typeof(bool))
                )
            )
            {
                result = ProvideValue(value, EnumDataTemplateKey);
            }
            else
            {
                result = ProvideValue(value, DefaultDataTemplateKey);
            }
        }
        return result;
    }

    private DataTemplate? ProvideValue(Property value, string templateKey)
    {
        DataTemplate? result;
        IServiceProvider sp = ServiceProviderCatcher.ServiceProvider!;
        IRootObjectProvider rop = sp.GetRequiredService<IRootObjectProvider>();
        string converterKey = $"{value.Name}Converter{Guid.NewGuid()}";
        (rop.RootObject as Window)!.Resources[converterKey] = new PropertyConverter { Property = value };
        string[] replaces = (Replaces is null ? Array.Empty<string>() : Replaces).Concat(new string[] { $"$converter:{converterKey}" }).ToArray();
        ParameterizedResourceExtension pre = new(templateKey)
        {
            Replaces = replaces
        };
        result = pre.ProvideValue(sp) as DataTemplate;
        (rop.RootObject as Window)!.Resources.Remove(converterKey);
        return result;
    }
}
