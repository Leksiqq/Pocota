using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota.Core;
using PocotaTestProject.Model;
using System.Diagnostics;

namespace PocotaTestProject;

public class TypesForestTests
{
    private IHost host;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
        Trace.AutoFlush = true;
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddPocotaCore(services =>
                {
                    services.AddTransient<IShipCall, ShipCall>();
                    services.AddTransient<IShipCallForListing, ShipCall>();
                    services.AddTransient<IShipCallAdditionalInfo, ShipCall>();
                    services.AddTransient<IArrivalShipCall, ShipCall>();
                    services.AddTransient<IDepartureShipCall, ShipCall>();
                    services.AddKeyMapping<ShipCall>(new Dictionary<string, Type> { { "ID_LINE", typeof(string) }, { "ID_ROUTE", typeof(int) } });

                    services.AddTransient<ILocation, Location>();
                    services.AddKeyMapping<Location>(new Dictionary<string, Type> { { "ID_LOCATION", typeof(string) } });

                    services.AddTransient<IRoute, Route>();
                    services.AddTransient<IRouteShort, Route>();
                    services.AddKeyMapping<Route>(new Dictionary<string, Type> { { "ID_LINE", typeof(string) }, { "ID_RHEAD", typeof(int) } });

                    services.AddTransient<ILine, Line>();
                    services.AddKeyMapping<Line>(new Dictionary<string, Type> { { "ID_LINE", typeof(string) } });

                    services.AddTransient<IVessel, Vessel>();
                    services.AddTransient<IVesselShort, Vessel>();
                    services.AddKeyMapping<Vessel>(new Dictionary<string, Type> { { "ID_VESSEL", typeof(string) } });

                    services.AddTransient<ITravelForListing, Travel>();
                });
            });
        host = hostBuilder.Build();
    }

    [Test]
    [TestCase(typeof(IShipCall))]
    [TestCase(typeof(IShipCallForListing))]
    [TestCase(typeof(IShipCallAdditionalInfo))]
    [TestCase(typeof(IArrivalShipCall))]
    [TestCase(typeof(IDepartureShipCall))]
    [TestCase(typeof(ILocation))]
    [TestCase(typeof(IRoute))]
    [TestCase(typeof(IRouteShort))]
    [TestCase(typeof(ILine))]
    [TestCase(typeof(IVessel))]
    [TestCase(typeof(IVesselShort))]
    [TestCase(typeof(ITravelForListing))]
    public void VisualTestValueRequests(Type type)
    {
        TypesForest tp = host.Services.GetRequiredService<TypesForest>();

        TypeNode typeTN = tp.GetTypeNode(type);

        int level = 0;
        typeTN.ValueRequests.ForEach(v => 
        {
            if(v.Kind is ValueRequestKind.Node)
            {
                level++;
            }
            level += v.PopsCount;
            Trace.WriteLine(v);
        });

        Assert.That(level, Is.EqualTo(0));
    }
}
