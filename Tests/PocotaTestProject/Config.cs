using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota;
using Net.Leksi.Pocota.Core;
using PocotaTestProject.Model;
using System.Diagnostics;

namespace PocotaTestProject;

public static class Config
{
    public static IHost Configure()
    {
        Trace.Listeners.Clear();
        Trace.Listeners.Add(new ConsoleTraceListener());
        Trace.AutoFlush = true;
        IHostBuilder hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddPocotaCore(services =>
                {
                    services.AddTransient<IShipCall, ShipCall>();
                    services.AddTransient<IShipCallForListing, ShipCall>();
                    services.AddTransient<IShipCallAdditionalInfo, ShipCall>();
                    services.AddTransient<IArrivalShipCall, ShipCall>();
                    services.AddTransient<IDepartureShipCall, ShipCall>();
                    services.AddPrimaryKey<ShipCall>(new Dictionary<string, Type> { { "ID_LINE", typeof(string) }, { "ID_ROUTE", typeof(int) } });

                    services.AddTransient<ILocation, Location>();
                    services.AddPrimaryKey<Location>(new Dictionary<string, Type> { { "ID_LOCATION", typeof(string) } });

                    services.AddTransient<IRoute, Route>();
                    services.AddTransient<IRouteShort, Route>();
                    services.AddPrimaryKey<Route>(new Dictionary<string, Type> { { "ID_LINE", typeof(string) }, { "ID_RHEAD", typeof(int) } });

                    services.AddTransient<ILine, Line>();
                    services.AddPrimaryKey<Line>(new Dictionary<string, Type> { { "ID_LINE", typeof(string) } });

                    services.AddTransient<IVessel, Vessel>();
                    services.AddTransient<IVesselShort, Vessel>();
                    services.AddPrimaryKey<Vessel>(new Dictionary<string, Type> { { "ID_VESSEL", typeof(string) } });

                    services.AddTransient<ITravelForListing, Travel>();
                });
                serviceCollection.AddPocoBuilder();
            });
        return hostBuilder.Build();
    }
}
