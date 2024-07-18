using ContosoPizza.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota.Client;
using Net.Leksi.Pocota.Client.UserControls;
using Net.Leksi.Util;
using Net.Leksi.WpfMarkup;
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
        //builder.Services.AddPocotaWpfApp<App>();
        builder.Services.AddPocotaWpfApp(s =>
        {
            App app = new();
            if (s.GetService<LifetimeObserver>() is LifetimeObserver lto)
            {
                app.Resources["LifetimeObserver"] = lto;
            }
            return app;
        });
        builder.Services.RemoveService(typeof(Localizer));
        builder.Services.AddSingleton<MyLocalizer>();
        builder.Services.AddSingleton<Localizer>(s => s.GetRequiredService<MyLocalizer>());
        builder.Services.RemoveService(typeof(INamesConverter));
        builder.Services.AddSingleton<INamesConverter, NamesConverter>();
        builder.Services.AddTransient<Window1>();
        builder.Services.AddLifetimeObserver(lto =>
        {
            lto.Trace<ObjectEditor>(true);
            lto.Trace<PropertyTemplateSelector>(true);
            lto.Trace<ParameterizedResourceExtension>(true);
            lto.Trace<MethodWindow>();
            lto.Trace<Window1>();
        });
        using IHost host = builder.Build();

        host.RunPocotaWpfApp(app =>
        {
            app.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
        });
    }
}
