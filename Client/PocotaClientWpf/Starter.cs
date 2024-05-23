using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Net.Leksi.Pocota.Client;

internal class Starter(IServiceProvider services)
{
    public void Start()
    {
        Application app = services.GetRequiredService<Application>();
        app.Run(services.GetRequiredKeyedService<Window>(PocotaWpfAppExtension.s_mainWindowServiceKey));
    }
}
