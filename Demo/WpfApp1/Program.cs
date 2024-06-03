using ContosoPizza.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota.Client;

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
        if(builder.Services.Where(sd => sd.ServiceType == typeof(Localizer)).FirstOrDefault() is ServiceDescriptor sd)
        {
            builder.Services.Remove(sd);
        }
        builder.Services.AddSingleton<MyLocalizer>();
        builder.Services.AddSingleton<Localizer>(s => s.GetRequiredService<MyLocalizer>());
        using IHost host = builder.Build();
        host.RunPocotaWpfApp(app =>
        {
            app.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
        });
    }
}
