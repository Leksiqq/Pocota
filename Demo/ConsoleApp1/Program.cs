using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

class Program
{
    [STAThread]
    public static void main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddHostedService<Starter>();
        builder.Services.AddTransient<MainWindow>();
        builder.Services.AddTransient<Application>();

        IHost host = builder.Build();

        host.Run();
    }
}


internal class Starter(IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        services.GetRequiredService<Application>().Run(services.GetRequiredService<MainWindow>());
        await Task.CompletedTask;
    }
}

