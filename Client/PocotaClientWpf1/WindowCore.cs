using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;
public class WindowCore
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
}
