using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        services.AddScoped(
            s =>
            {
                Application app = createApplication.Invoke(s);
                app.Resources[ServiceProviderResourceKey] = s;
                app.Resources[LocalizerResourceKey] = s.GetRequiredService<Localizer>();
                app.Resources[NamesConverterResourceKey] = s.GetRequiredService<INamesConverter>();
                JsonSerializerOptions commonJSO = new()
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                commonJSO.Converters.Add(new CommonJsonConverterFactory());
                app.Resources[CommonJsonSerializerOptionsResourceKey] = commonJSO;
                return app;
            }
        );
       services.AddSingleton<Localizer>();

        ConnectorsMethodsList methods = new();
        foreach(ServiceDescriptor sd in services)
        {
            if (typeof(Connector).IsAssignableFrom(sd.ServiceType))
            {
                methods.AddConnectorType(sd.ServiceType);
            }
        }
        services.AddSingleton(s => {
            methods.Services = s;
            return methods;
        });
        services.AddSingleton<INamesConverter, StubNamesConverter>();
        return services;
    }
    public static IServiceCollection AddPocotaWpfApp<TApplication>(
        this IServiceCollection services,
        Type? mainWindowType = null
    )
        where TApplication : Application, new()
    {
        return AddPocotaWpfApp(services, s => new TApplication(), mainWindowType);
    }
    public static IServiceCollection AddPocotaWpfApp<TApplication, TWindow>(
        this IServiceCollection services
    )
        where TApplication : Application, new()
        where TWindow: Window, new()
    {
        return AddPocotaWpfApp(services, s => new TApplication(), typeof(TWindow));
    }
    public static IServiceProvider GetServiceProvider(this Application app)
    {
        return app.Resources[ServiceProviderResourceKey] as IServiceProvider ?? throw new NullReferenceException();
    }
    public static IServiceCollection RemoveService(this IServiceCollection services, Type serviceType)
    {
        if (services.Where(sd => sd.ServiceType == serviceType).FirstOrDefault() is ServiceDescriptor sd)
        {
            services.Remove(sd);
        }
        return services;
    }
    public static void RunPocotaWpfApp(this IHost host, Action<Application>? config = null)
    {
        Application app = host.Services.GetRequiredService<Application>();
        config?.Invoke(app);
        app.Run(host.Services.GetRequiredKeyedService<Window>(s_mainWindowServiceKey));
    }
}
