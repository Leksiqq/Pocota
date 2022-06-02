using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota.Core;
using PocotaTestProject.Model;
using System.Diagnostics;

namespace PocotaTestProject;

public class TypesForestTest
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Trace.Listeners.Clear();
        Trace.Listeners.Add(new ConsoleTraceListener());
        Trace.AutoFlush = true;
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
    [TestCase(typeof(ShipCall))]
    public void VisualTestValueRequests(Type type)
    {
        IHost host = Config.Configure();
        TypesForest tp = host.Services.GetRequiredService<TypesForest>();

        TypeNode typeTN = tp.GetTypeNode(type);

        typeTN.ValueRequests.ForEach(v => 
        {
            Trace.WriteLine(v);
            Assert.That(v.Path.Split(new[] {'/' }, StringSplitOptions.RemoveEmptyEntries).Length, Is.EqualTo(v.Level));
        });
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
    [TestCase(typeof(ShipCall))]
    public void VisualTestTreeToString(Type type)
    {
        IHost host = Config.Configure1();

        object obj = host.Services.GetRequiredService(type);
        TypesForest tp = host.Services.GetRequiredService<TypesForest>();

        Trace.WriteLine(tp.TreeToString(obj, type));

    }

}


