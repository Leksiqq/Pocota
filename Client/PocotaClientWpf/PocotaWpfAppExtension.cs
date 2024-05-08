using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Net.Leksi.Pocota.Client;

public static class PocotaWpfAppExtension
{
    internal const string s_mainWindowServiceKey = "Net.Leksi.Pocota.Client.MainWindow";

    public static IServiceCollection AddPocotaWpfApp(
        this IServiceCollection services, 
        Func<IServiceProvider,Application> createApplication, 
        Type? mainWindowType = null,
#pragma warning disable IDE0060 // Удалите неиспользуемый параметр
        Type? touch = null
#pragma warning restore IDE0060 // Удалите неиспользуемый параметр
    )
    {
        services.AddHostedService<Starter>();
        if(mainWindowType is { })
        {
            services.AddKeyedTransient(
                s_mainWindowServiceKey, 
                (s, o) => (Window)Activator.CreateInstance(mainWindowType)!
            );
        }
        else
        {
            services.AddKeyedTransient<Window>(
                s_mainWindowServiceKey,
                (s, o) => new DefaultMainWindow()
            );
        }
        services.AddScoped<Application>(
            s =>
            {
                Application app = createApplication.Invoke(s);
                app.Resources["ServiceProvider"] = s;
                I18nConverter i18NConverter = new();
                i18NConverter.AddResourceManager(Properties.Resources.ResourceManager);
                app.Resources["I18nConverter"] = i18NConverter;
                return app;
            }
        );
        services.AddScoped<WindowsList>();
        return services;
    }
    public static IServiceCollection AddPocotaWpfApp<TApplication>(
        this IServiceCollection services,
        Type? mainWindowType = null,
        Type? touch = null
    )
        where TApplication : Application, new()
    {
        return AddPocotaWpfApp(services, s => new TApplication(), mainWindowType, touch);
    }
    public static IServiceCollection AddPocotaWpfApp<TApplication, TWindow>(
        this IServiceCollection services,
        Type? touch = null
    )
        where TApplication : Application, new()
        where TWindow: Window, new()
    {
        return AddPocotaWpfApp(services, s => new TApplication(), typeof(TWindow), touch);
    }
}
