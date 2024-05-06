using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Net.Leksi.Pocota.Client;

internal class Starter(IServiceProvider services) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            Application app = services.GetRequiredService<Application>();
            app.Run(services.GetRequiredKeyedService<Window>(PocotaWpfAppExtension.s_mainWindowServiceKey));
        }
        finally
        {
            await services.GetRequiredService<IHost>().StopAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

    }
}
