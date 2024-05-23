using System.Windows;
using System.Windows.Markup;

namespace Net.Leksi.Pocota.Client;

[MarkupExtensionReturnType(typeof(Localizer))]
public class LocalizerExtension : MarkupExtension
{
    private readonly Localizer _localizer;
    public LocalizerExtension()
    {
        _localizer = (Localizer)Application.Current.Resources["Localizer"];
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        Console.WriteLine($"_localizer: {_localizer}");
        return _localizer;
    }
}
