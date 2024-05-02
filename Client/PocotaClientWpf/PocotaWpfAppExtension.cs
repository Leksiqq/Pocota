using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Net.Leksi.Pocota.Client;

public static class PocotaWpfAppExtension
{
    internal const string s_mainWindowServiceKey = "Net.Leksi.Pocota.Client.MainWindow";
    internal const string s_applicationServiceKey = "Net.Leksi.Pocota.Client.Application";

    public static IServiceCollection AddPocotaWpfApp(this IServiceCollection services, Type applicationType, Type? mainWindowType = null, Type? touch = null)
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
        services.AddKeyedScoped(
            s_applicationServiceKey,
            (s, o) => (Application)Activator.CreateInstance(applicationType, s)!
        );
        return services;
    }
    public static IServiceCollection AddPocotaWpfApp<TApplication>(this IServiceCollection services, Type? mainWindowType = null, Type? touch = null)
    {
        return AddPocotaWpfApp(services, typeof(TApplication), mainWindowType, touch);
    }
    public static IServiceCollection AddPocotaWpfApp<TApplication, TMainWindow>(this IServiceCollection services, Type? touch = null)
    {
        return AddPocotaWpfApp(services, typeof(TApplication), typeof(TMainWindow), touch);
    }
}
