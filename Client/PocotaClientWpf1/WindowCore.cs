using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;
public class WindowCore: IValueConverter
{
    private readonly Window _owner;
    public ApplicationCore ApplicationCore => Services.GetRequiredService<ApplicationCore>();
    public IServiceProvider Services => (IServiceProvider)Application.Current.Resources[ServiceProviderResourceKey];
    internal WindowCore(Window owner)
    {
        _owner = owner;
        _owner.Closed += _owner_Closed;
        Services.GetRequiredService<ApplicationCore>().Touch();
    }
    private void _owner_Closed(object? sender, EventArgs e)
    {
        Services.GetRequiredService<ApplicationCore>().Touch();
    }

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("ThisWindow".Equals(parameter))
        {
            return value == _owner;
        }
        if ("AdditionalInfo".Equals(parameter))
        {
            return value == _owner.Owner ? $" - {(Application.Current.Resources[LocalizerResourceKey] as Localizer)?.Owner}" : ((value as Window)?.Owner == _owner ? $" - {(Application.Current.Resources[LocalizerResourceKey] as Localizer)?.Owned}" : string.Empty);
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
