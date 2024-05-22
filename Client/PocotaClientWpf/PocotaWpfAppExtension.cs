using Microsoft.Extensions.DependencyInjection;
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
        Type? mainWindowType = null,
#pragma warning disable IDE0060 // Удалите неиспользуемый параметр
        Type? touch = null,
#pragma warning restore IDE0060 // Удалите неиспользуемый параметр
        bool showLabels = false
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
        services.AddLocalization();
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
                (s, o) => new MethodsWindow()
            );
        }
        services.AddScoped<Application>(
            s =>
            {
                Application app = createApplication.Invoke(s);
                app.Resources[ServiceProvider] = s;
                I18nConverter i18NConverter = s.GetRequiredService<I18nConverter>();
                if(showLabels is { })
                {
                    i18NConverter.ShowLabels = showLabels;
                }
                i18NConverter.AddResourceManager(Properties.Resources.ResourceManager);
                app.Resources["I18nConverter"] = i18NConverter;
                return app;
            }
        );
        services.AddScoped<I18nConverter>();
        services.AddScoped<WindowsList>();
        services.AddScoped<Trans>();

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
