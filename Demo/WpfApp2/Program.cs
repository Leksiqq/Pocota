using ContosoPizza.Models.Client;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota.Client;

namespace WpfApp2;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddPocotaWpfApp<App, MainWindow>(touch: typeof(Pizza));

        IHost host = builder.Build();

        host.Run();
    }
}
