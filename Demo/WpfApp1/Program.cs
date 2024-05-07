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
        host.Run();
    }
}
