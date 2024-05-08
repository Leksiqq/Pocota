using ContosoPizza.Models.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota.Client;
using System.Windows;

namespace WpfApp1;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddPocotaWpfApp<App>(touch: typeof(Pizza));

        IHost host = builder.Build();
        if (host.Services.GetRequiredService<Application>().Resources["I18nConverter"] is I18nConverter conv)
        {
            conv.AddResourceManager(WpfApp1.Properties.Resources.ResourceManager);
        }

        host.Run();
    }
}
