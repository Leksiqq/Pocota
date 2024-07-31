using ContosoPizza.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota.Client;
using Net.Leksi.Pocota.Client.UserControls;
using Net.Leksi.Util;
using Net.Leksi.WpfMarkup;
using System.Windows;

namespace WpfApp1;

static class Program
{
    private static Timer? s_timer;
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
        builder.Services.AddLifetimeObserver(lto =>
        {
            //PocotaClientWpf1
            lto.Trace<ObjectEditor>(true);
            lto.Trace<PropertyTemplateSelector>(true);
            lto.Trace<MethodWindow>();
            lto.Trace<ObjectWindow>();

            //WpfMarkupExtension
            lto.Trace<BindingProxy>(true);
            lto.Trace<BindingProxyMarkup>(true);
            lto.Trace<BoolExpressionConverter>(true);
            lto.Trace<ConverterProxy>(true);
            lto.Trace<DataGridManager>(true);
            lto.Trace<DataSwitch>(true);
            lto.Trace<ParameterizedResourceExtension>(true);
            lto.Trace<SortByColumn>(true);
            lto.Trace<SortByColumnArgs>(true);
            lto.Trace<SortByColumnConverter>(true);
            lto.Trace<StyleCombiner>(true);
            lto.Trace<Unsort>(true);
            lto.Trace<XamlServiceProviderCatcher>(true);
        }, true);
        using IHost host = builder.Build();

        host.RunPocotaWpfApp(app =>
        {
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            if (app.Resources["LifetimeObserver"] is LifetimeObserver lto)
            {
                lto.NextTracedCount += Lto_NextTracedCount;
                LifetimeVisualizer.Instance.Start(lto!);
                Net.Leksi.WpfMarkup.NotifyInstanceCreated.InstanceCreated += (s, e) =>
                {
                    lto!.TraceObject(s!);
                };
                Net.Leksi.Pocota.Client.NotifyInstanceCreated.InstanceCreated += (s, e) =>
                {
                    lto!.TraceObject(s!);
                };
            }

            app.GetApplicationCore().NeedToCheckAndShowLeaks = true;

            WpfSpy.Instance.Start(RandomMethodWindowShower.Instance);
        });
    }

    private static void Lto_NextTracedCount(object? sender, EventArgs e)
    {
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true);
        GC.WaitForPendingFinalizers();
    }
}
