using ContosoPizza.Client;
using ContosoPizza.Models.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota.Client;
using System.Windows;

namespace WpfApp1;

static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddPocotaWpfApp<App>(touch: typeof(Pizza));
        builder.Services.AddPizza();
        IHost host = builder.Build();
        if (host.Services.GetRequiredService<Application>().Resources[Constants.I18nConverter] is I18nConverter conv)
        {
            conv.AddResourceManager(WpfApp1.Properties.Resources.ResourceManager);
        }
        host.Services.GetRequiredService<PizzaConnector>().BaseAddress = new Uri("http://localhost:5000/Pizza/");
        host.Services.GetRequiredService<PizzaConnector>().GetPocotaConfigAsync(CancellationToken.None).Wait();
        host.Run();
    }
}
