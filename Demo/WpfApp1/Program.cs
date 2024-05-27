﻿using ContosoPizza.Client;
using ContosoPizza.Models.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Net.Leksi.Pocota.Client;
using System.Globalization;
using System.Windows;

namespace WpfApp1;

static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        //CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddPocotaWpfApp<App>(touch: typeof(Pizza));
        builder.Services.AddPizza();
        if(builder.Services.Where(sd => sd.ServiceType == typeof(Localizer)).FirstOrDefault() is ServiceDescriptor sd)
        {
            builder.Services.Remove(sd);
        }
        builder.Services.AddSingleton<MyLocalizer>();
        builder.Services.AddSingleton<Localizer>(s => s.GetRequiredService<MyLocalizer>());
        using IHost host = builder.Build();
        host.Services.GetRequiredService<PizzaConnector>().BaseAddress = new Uri("http://localhost:5000/Pizza/");
        host.Services.GetRequiredService<PizzaConnector>().GetPocotaConfigAsync(CancellationToken.None).Wait();
        host.RunPocotaWpfApp();
    }
}
