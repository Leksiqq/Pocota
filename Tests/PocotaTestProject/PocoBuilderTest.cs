using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota;
using PocotaTestProject.Model;

namespace PocotaTestProject;

public class PocoBuilderTest
{
    private IHost _host;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _host = Config.Configure();
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
    public void VisualTestGenerateHandlerSkeleton(Type type)
    {
        PocoBuilder builder = _host.Services.GetRequiredService<PocoBuilder>();

        string skeleton = builder.GenerateHandlerSkeleton(type);

        Console.WriteLine(skeleton);
    }

}
