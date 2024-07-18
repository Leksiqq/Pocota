using Net.Leksi.Util;
using Net.Leksi.WpfMarkup;
using System.Windows;
using System.Windows.Controls;

namespace Net.Leksi.Pocota.Client;

public class PropertyTemplateSelector: DataTemplateSelector
{
    public IServiceProvider? ServiceProvider { get; set; }
    public string ClassDataTemplateKey { get; set; } = null!;
    public string EnumDataTemplateKey { get; set; } = null!;
    public string TextDataTemplateKey { get; set; } = null!;
    public PropertyTemplateSelector() 
    {
        if (Application.Current.TryFindResource("LifetimeObserver") is LifetimeObserver lto)
        {
            lto.TraceObject(this);
        }
    }
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        DataTemplate? result = null;
        if (item is Field field)
        {
            if(field.EntityProperty?.Access is Contract.AccessKind.NotSet || field.EntityProperty?.Access is Contract.AccessKind.Forbidden)
            {
                result = ProvideValue(ClassDataTemplateKey);
                
            }
            //else if (property is ListProperty)
            //{
            //}
            else if (field.Type.IsClass && field.Type != typeof(string))
            {
                result = ProvideValue(ClassDataTemplateKey);
            }
            else if (
                (field.Type.IsEnum || field.Type == typeof(bool))
                || (
                    field.Type.IsGenericType
                    && field.Type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && (field.Type.GetGenericArguments()[0].IsEnum || field.Type.GetGenericArguments()[0] == typeof(bool))
                )
            )
            {
                result = ProvideValue(EnumDataTemplateKey);
            }
            else
            {
                result = ProvideValue(TextDataTemplateKey);
            }
            if (result != null)
            {
                return result;
            }
        }
        return base.SelectTemplate(item, container);
    }
    private DataTemplate? ProvideValue(string templateKey)
    {
        DataTemplate? result = null;
        if (ServiceProvider != null)
        {
            ParameterizedResourceExtension pre = new(templateKey);
            result = pre.ProvideValue(ServiceProvider) as DataTemplate;
            ServiceProvider = null;
        }
        return result;
    }
}
