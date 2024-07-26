using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Markup;
namespace Net.Leksi.Pocota.Client;
public static class PocotaWpfAppExtension
{
    private const string s_mainWindowServiceKey = "Net.Leksi.Pocota.Client.MainWindow";
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
        if (mainWindowType != null)
        {
            services.AddKeyedTransient(
                s_mainWindowServiceKey, 
                (s, o) => {
                    Window w = (Window)Activator.CreateInstance(mainWindowType)!;
                    Application.Current.GetApplicationCore().AttachWindow(w);
                    return w;
                }
            );
        }
        else
        {
            services.AddKeyedTransient(
                s_mainWindowServiceKey,
                (s, o) => {
                    Window w = new MethodsWindow();
                    Application.Current.GetApplicationCore().AttachWindow(w);
                    return w;
                }
            );
        }
        services.AddScoped(
            s =>
            {
                Application app = createApplication.Invoke(s);
                app.Resources[Constants.ServiceProvider] = s;
                app.Resources[Constants.Localizer] = s.GetRequiredService<Localizer>();
                app.Resources[Constants.NamesConverter] = s.GetRequiredService<INamesConverter>();
                JsonSerializerOptions commonJSO = new()
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                commonJSO.Converters.Add(new CommonJsonConverterFactory());
                app.Resources[Constants.CommonJsonSerializerOptions] = commonJSO;
                app.Resources[Constants.ApplicationCore] = new ApplicationCore();
                app.Resources[Constants.PriorInfo] = Constants.PriorInfo;
                app.Resources[Constants.Title] = Constants.Title;
                app.Resources[Constants.AdditionalInfo] = Constants.AdditionalInfo;
                app.Resources[Constants.ThisWindow] = Constants.ThisWindow;
                return app;
            }
        );
        services.AddTransient<Localizer>();
        services.AddTransient<MethodWindow>();
        services.AddTransient<ObjectWindow>();
        services.AddTransient<WindowsWindow>();

        ConnectorsMethodsList methods = new();
        List<ServiceDescriptor> pocotaContextDescriptors = [];
        foreach(ServiceDescriptor sd in services)
        {
            if (typeof(Connector).IsAssignableFrom(sd.ServiceType))
            {
                methods.AddConnectorType(sd.ServiceType);
            }
        }
        foreach(ServiceDescriptor sd in pocotaContextDescriptors)
        {
            services.Add(sd);
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
    public static IServiceProvider GetServiceProvider(this Application app) => 
        (app.Resources[Constants.ServiceProvider] as IServiceProvider)!;
    public static Localizer GetLocalizer(this Application app) => 
        app.GetServiceProvider().GetRequiredService<Localizer>();
    public static INamesConverter GetNamesConverter(this Application app) =>
        app.GetServiceProvider().GetRequiredService<INamesConverter>();
    public static ApplicationCore GetApplicationCore(this Application app) =>
        (app.Resources[Constants.ApplicationCore] as ApplicationCore)!;
    public static JsonSerializerOptions GetCommonJsonSerializerOptions(this Application app) =>
        (app.Resources[Constants.CommonJsonSerializerOptions] as JsonSerializerOptions)!;
    public static void AttachWindow(this Application app, Window window, Window? launcher = null) => 
        GetApplicationCore(app).AttachWindow(window, launcher);
    public static T GetRequiredService<T>(this Application app) where T: notnull => GetServiceProvider(app).GetRequiredService<T>();
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
