using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Net.Leksi.Pocota.Client;

internal class Starter(IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            Application app = services.GetRequiredKeyedService<Application>(PocotaWpfAppExtension.s_applicationServiceKey);
            app.Run(services.GetRequiredKeyedService<Window>(PocotaWpfAppExtension.s_mainWindowServiceKey));
        }
        finally
        {
            await services.GetRequiredService<IHost>().StopAsync(stoppingToken);
        }
    }
}
