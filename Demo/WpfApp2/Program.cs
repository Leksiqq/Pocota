using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Util;
using System.Windows;

namespace WpfApp2;

static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddTransient<MainWindow>();
        builder.Services.AddTransient<Window1>();
        builder.Services.AddTransient<Application>(services =>
        {
            App app = new();
            app.Resources["ServiceProvider"] = services;
            return app;
        });
        builder.Services.AddLifetimeObserver(lto =>
        {
            lto.Trace<MainWindow>();
            lto.Trace<Window1>();
        });
        IHost host = builder.Build();
        Application app = host.Services.GetRequiredService<Application>();
        LifetimeVisualizer.Start(host.Services.GetRequiredService<LifetimeObserver>());
        WpfSpy.Instance.Start(new SpyRunner());
        app.Run(host.Services.GetRequiredService<MainWindow>());
    }
}
