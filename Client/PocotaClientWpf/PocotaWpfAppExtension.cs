﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;

public static class PocotaWpfAppExtension
{
    internal const string s_mainWindowServiceKey = "Net.Leksi.Pocota.Client.MainWindow";

    public static IServiceCollection AddPocotaWpfApp(
        this IServiceCollection services, 
        Func<IServiceProvider,Application> createApplication, 
        Type? mainWindowType = null
    )
    {
        #region https://serialseb.com/blog/2007/04/03/wpf-tips-1-have-all-your-dates-times/
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
            )
        );
        #endregion
        services.AddLocalization(op => op.ResourcesPath = "Properties");
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
                (s, o) => new MethodsWindow()
            );
        }
        services.AddScoped<Application>(
            s =>
            {
                Application app = createApplication.Invoke(s);
                app.Resources[ServiceProvider] = s;
                app.Resources["Localizer"] = s.GetRequiredService<Localizer>();
                return app;
            }
        );
        services.AddScoped<WindowsList>();
        services.AddSingleton<Localizer>();

        ConnectorsMethodsList methods = new();
        foreach(ServiceDescriptor sd in services)
        {
            if (typeof(Connector).IsAssignableFrom(sd.ServiceType))
            {
                methods.AddConnectorType(sd.ServiceType);
            }
        }
        services.AddSingleton(methods);
        return services;
    }
    public static IServiceCollection AddPocotaWpfApp<TApplication>(
        this IServiceCollection services,
        Type? mainWindowType = null,
        Type? touch = null
    )
        where TApplication : Application, new()
    {
        return AddPocotaWpfApp(services, s => new TApplication(), mainWindowType);
    }
    public static IServiceCollection AddPocotaWpfApp<TApplication, TWindow>(
        this IServiceCollection services,
        Type? touch = null
    )
        where TApplication : Application, new()
        where TWindow: Window, new()
    {
        return AddPocotaWpfApp(services, s => new TApplication(), typeof(TWindow));
    }
    public static void RunPocotaWpfApp(this IHost host)
    {
        Application app = host.Services.GetRequiredService<Application>();
        app.Run(host.Services.GetRequiredKeyedService<Window>(s_mainWindowServiceKey));
    }
}
