using ContosoPizza.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota.Client;
using System.Diagnostics;

namespace WpfApp1;

static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        //CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddPizza(baseUri: new Uri("http://localhost:5000/Pizza/"));
        builder.Services.AddPocotaWpfApp<App>();
        builder.Services.RemoveService(typeof(Localizer));
        builder.Services.AddSingleton<MyLocalizer>();
        builder.Services.AddSingleton<Localizer>(s => s.GetRequiredService<MyLocalizer>());
        builder.Services.RemoveService(typeof(INamesConverter));
        builder.Services.AddSingleton<INamesConverter, NamesConverter>();
        using IHost host = builder.Build();
        Process currentProcess = Process.GetCurrentProcess();

        // Set the maximum working set size (in bytes)
        long maxWorkingSetBytes = 1024 * 1024 * 50; // 100 MB
        currentProcess.MaxWorkingSet = new IntPtr(maxWorkingSetBytes);
        host.RunPocotaWpfApp(app =>
        {
            app.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
        });
    }
}
