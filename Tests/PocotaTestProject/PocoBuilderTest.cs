using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota;
using Net.Leksi.Pocota.Core;
using PocotaTestProject.Model;
using System.Collections;
using System.Diagnostics;

namespace PocotaTestProject;

public class PocoBuilderTest
{
    private IHost _host;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Trace.Listeners.Clear();
        Trace.Listeners.Add(new ConsoleTraceListener());
        Trace.AutoFlush = true;
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

    [Test]
    public void TestBuild()
    {
        int shipCallsCount = 10;

        IHost host1 = Config.Configure1();
        TypesForest tf1 = host1.Services.GetRequiredService<TypesForest>();
        List<ShipCall> shipCalls = new();
        for (int i = 0; i < shipCallsCount; ++i)
        {
            shipCalls.Add(host1.Services.GetRequiredService<ShipCall>());
        }
        Dictionary<Type, HashSet<int>> objectsCounts = CountObjects(tf1, shipCalls);

        Assert.That(objectsCounts[typeof(ShipCall)].Count, Is.EqualTo(shipCallsCount * 2));
        Assert.That(objectsCounts[typeof(Location)].Count, Is.EqualTo(shipCallsCount * 2));
        Assert.That(objectsCounts[typeof(Line)].Count, Is.EqualTo(shipCallsCount));
        Assert.That(objectsCounts[typeof(Vessel)].Count, Is.EqualTo(shipCallsCount));
        Assert.That(objectsCounts[typeof(Route)].Count, Is.EqualTo(shipCallsCount));

        List<ShipCall> shipCalls1 = new();

        CallShipBuildHandler buildHandler = new(host1.Services);
        PocoBuilder builder = _host.Services.GetRequiredService<PocoBuilder>();

        foreach (ShipCall source in shipCalls)
        {
            buildHandler.DataSource = source;
            shipCalls1.Add(builder.Build<ShipCall>(buildHandler.Handler));
        }

        objectsCounts = CountObjects(_host.Services.GetRequiredService<TypesForest>(), shipCalls1, true);

    }

    private Dictionary<Type, HashSet<int>> CountObjects(TypesForest typesForest, IEnumerable values, bool verbose = false)
    {
        Dictionary<Type, HashSet<int>> objectsCounts = new();

        foreach (object item in values)
        {
            typesForest.WalkTree(item, typeof(ShipCall), _ => { }, _ => { }, afterNode: args =>
            {
                object? obj = args.Value;
                if (obj is { })
                {
                    if (!objectsCounts.ContainsKey(obj.GetType()))
                    {
                        objectsCounts.Add(obj.GetType(), new HashSet<int>());
                    }
                    objectsCounts[obj.GetType()].Add(obj.GetHashCode());
                    if(verbose)
                    {
                        Trace.WriteLine($"{obj.GetType()}: {obj.GetHashCode()}");
                    }
                }
            });
        }
        if (verbose)
        {
            Trace.WriteLine("objects counts");
            foreach (var entry in objectsCounts)
            {
                Trace.WriteLine($"{entry.Key}: {entry.Value.Count}");
            }
        }
        return objectsCounts;
    }

    internal class CallShipBuildHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private KeyRing? keyRing = null;

        internal ShipCall DataSource { get; set; }

        internal ValueNodeEventHandler Handler { get; init; }

        internal CallShipBuildHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Handler = (ValueNodeEventArgs args) =>
            {
                switch (args.Path)
                {
                    case "/":
                        keyRing = _serviceProvider.GetRequiredService<Container>().GetKeyRing(DataSource);
                        break;
                    case "/ID_LINE":
                        args.Value = keyRing["ID_LINE"];
                        break;
                    case "/ID_ROUTE":
                        args.Value = keyRing["ID_ROUTE"];
                        break;
                    case "/ActualArrival":
                        args.Value = DataSource.ActualArrival;
                        break;
                    case "/ActualDeparture":
                        args.Value = DataSource.ActualDeparture;
                        break;
                    case "/AdditionalInfo":
                        args.Value = DataSource.AdditionalInfo;
                        break;
                    case "/Condition":
                        args.Value = DataSource.Condition;
                        break;
                    case "/ScheduledArrival":
                        args.Value = DataSource.ScheduledArrival;
                        break;
                    case "/ScheduledDeparture":
                        args.Value = DataSource.ScheduledDeparture;
                        break;
                    case "/Voyage":
                        args.Value = DataSource.Voyage;
                        break;
                    case "/VoyageAlt":
                        args.Value = DataSource.VoyageAlt;
                        break;
                    case "/Location":
                        keyRing = _serviceProvider.GetRequiredService<Container>().GetKeyRing(DataSource.Location);
                        break;
                    case "/Location/ID_LOCATION":
                        args.Value = keyRing["ID_LOCATION"];
                        break;
                    case "/Location/Name":
                        args.Value = DataSource.Location.Name;
                        break;
                    case "/Location/ShortName":
                        args.Value = DataSource.Location.ShortName;
                        break;
                    case "/Location/Type":
                        args.Value = DataSource.Location.Type;
                        break;
                    case "/Location/Unlocode":
                        args.Value = DataSource.Location.Unlocode;
                        break;
                    case "/PrevCall":
                        if (DataSource.Location is { })
                        {
                            keyRing = _serviceProvider.GetRequiredService<Container>().GetKeyRing(DataSource.PrevCall);
                        }
                        else
                        {
                            args.Value = null;
                        }
                        break;
                    case "/PrevCall/ID_LINE":
                        args.Value = keyRing["ID_LINE"];
                        break;
                    case "/PrevCall/ID_ROUTE":
                        args.Value = keyRing["ID_ROUTE"];
                        break;
                    case "/Route":
                        keyRing = _serviceProvider.GetRequiredService<Container>().GetKeyRing(DataSource.Route);
                        break;
                    case "/Route/ID_LINE":
                        args.Value = keyRing["ID_LINE"];
                        break;
                    case "/Route/ID_RHEAD":
                        args.Value = keyRing["ID_RHEAD"];
                        break;
                    case "/Route/Line":
                        keyRing = _serviceProvider.GetRequiredService<Container>().GetKeyRing(DataSource.Route.Line);
                        break;
                    case "/Route/Line/ID_LINE":
                        args.Value = keyRing["ID_LINE"];
                        break;
                    case "/Route/Line/Description":
                        args.Value = DataSource.Route.Line.Description;
                        break;
                    case "/Route/Line/Name":
                        args.Value = DataSource.Route.Line.Name;
                        break;
                    case "/Route/Line/ShortName":
                        args.Value = DataSource.Route.Line.ShortName;
                        break;
                    case "/Route/Vessel":
                        keyRing = _serviceProvider.GetRequiredService<Container>().GetKeyRing(DataSource.Route.Vessel);
                        break;
                    case "/Route/Vessel/ID_VESSEL":
                        args.Value = keyRing["ID_VESSEL"];
                        break;
                    case "/Route/Vessel/Brutto":
                        args.Value = DataSource.Route.Vessel.Brutto;
                        break;
                    case "/Route/Vessel/CallSign":
                        args.Value = DataSource.Route.Vessel.CallSign;
                        break;
                    case "/Route/Vessel/Description":
                        args.Value = DataSource.Route.Vessel.Description;
                        break;
                    case "/Route/Vessel/Height":
                        args.Value = DataSource.Route.Vessel.Height;
                        break;
                    case "/Route/Vessel/IsOcean":
                        args.Value = DataSource.Route.Vessel.IsOcean;
                        break;
                    case "/Route/Vessel/Length":
                        args.Value = DataSource.Route.Vessel.Length;
                        break;
                    case "/Route/Vessel/LineMeters":
                        args.Value = DataSource.Route.Vessel.LineMeters;
                        break;
                    case "/Route/Vessel/Name":
                        args.Value = DataSource.Route.Vessel.Name;
                        break;
                    case "/Route/Vessel/Netto":
                        args.Value = DataSource.Route.Vessel.Netto;
                        break;
                    case "/Route/Vessel/RiffCount":
                        args.Value = DataSource.Route.Vessel.RiffCount;
                        break;
                    case "/Route/Vessel/Width":
                        args.Value = DataSource.Route.Vessel.Width;
                        break;
                    case "/Route/Vessel/Port":
                        if (DataSource.Route.Vessel.Port is { })
                        {
                            keyRing = _serviceProvider.GetRequiredService<Container>().GetKeyRing(DataSource.Route.Vessel.Port);
                        }
                        else
                        {
                            args.Value = null;
                        }
                        break;
                    case "/Route/Vessel/Port/ID_LOCATION":
                        args.Value = keyRing["ID_LOCATION"];
                        break;
                    case "/Route/Vessel/Port/Name":
                        args.Value = DataSource.Route.Vessel.Port.Name;
                        break;
                    case "/Route/Vessel/Port/ShortName":
                        args.Value = DataSource.Route.Vessel.Port.ShortName;
                        break;
                    case "/Route/Vessel/Port/Type":
                        args.Value = DataSource.Route.Vessel.Port.Type;
                        break;
                    case "/Route/Vessel/Port/Unlocode":
                        args.Value = DataSource.Route.Vessel.Port.Unlocode;
                        break;
                }
            };
        }

    }
}
