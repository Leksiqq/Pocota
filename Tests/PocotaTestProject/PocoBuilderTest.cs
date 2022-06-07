using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota;
using Net.Leksi.Pocota.Core;
using PocotaTestProject.Model;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using static PocotaTestProject.Config;

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
        Config.ModelObjectFactory modelObjectFactory = _host.Services.GetRequiredService<Config.ModelObjectFactory>();

        Dictionary<Type, HashSet<int>> objectsCounts;
        List<ShipCall> shipCalls = new();
        for (int i = 0; i < shipCallsCount; ++i)
        {
            shipCalls.Add(modelObjectFactory.Create());
        }
        objectsCounts = CountObjects(_host.Services, shipCalls, true);

        Assert.That(objectsCounts[typeof(ShipCall)].Count, Is.EqualTo(shipCallsCount * 2));
        Assert.That(objectsCounts[typeof(Location)].Count, Is.EqualTo(shipCallsCount * 4));
        Assert.That(objectsCounts[typeof(Line)].Count, Is.EqualTo(shipCallsCount * 2));
        Assert.That(objectsCounts[typeof(Vessel)].Count, Is.EqualTo(shipCallsCount * 2));
        Assert.That(objectsCounts[typeof(Route)].Count, Is.EqualTo(shipCallsCount * 2));

        List<ShipCall> shipCalls1 = new();

        DbDataReader dataReader = new ModelDataReader();
        CallShipBuildHandler buildHandler = new(dataReader);
        PocoBuilder builder = _host.Services.GetRequiredService<PocoBuilder>();


        while (dataReader.Read())
        {
            shipCalls1.Add(builder.Build<ShipCall>(buildHandler.Handler));
        }

        objectsCounts = CountObjects(_host.Services, shipCalls1, true);
        Assert.That(objectsCounts[typeof(ShipCall)].Count, Is.EqualTo(shipCallsCount  + 1));
        Assert.That(objectsCounts[typeof(Location)].Count, Is.EqualTo(5));
        Assert.That(objectsCounts[typeof(Line)].Count, Is.EqualTo(1));
        Assert.That(objectsCounts[typeof(Vessel)].Count, Is.EqualTo(1));
        Assert.That(objectsCounts[typeof(Route)].Count, Is.EqualTo(1));


    }



    private Dictionary<Type, HashSet<int>> CountObjects(IServiceProvider serviceProvider, IEnumerable values, bool verbose = false)
    {
        TypesForest typesForest = serviceProvider.GetRequiredService<TypesForest>();
        Container container = serviceProvider.GetRequiredService<Container>();
        Dictionary<Type, HashSet<int>> objectsCounts = new();
        Dictionary<Type, List<object>> objects = new();

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
                        objects.Add(obj.GetType(), new List<object>());
                    }
                    if (objectsCounts[obj.GetType()].Add(obj.GetHashCode()))
                    {
                        objects[obj.GetType()].Add(obj);
                    }
                    if (verbose)
                    {
                        //Trace.WriteLine($"{obj.GetType()}: {obj.GetHashCode()} {string.Join(", ", container.GetKeyRing(obj).Select(v => v.Key + ":" + v.Value))}");
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

            //foreach(var obj in objects[typeof(ShipCall)])
            //{
            //    Trace.WriteLine(typesForest.TreeToString(obj));
            //}
        }
        return objectsCounts;
    }

    internal class CallShipBuildHandler
    {
        private readonly DbDataReader _dataReader;

        internal ValueNodeEventHandler Handler { get; init; }

        internal CallShipBuildHandler(DbDataReader dataReader)
        {
            _dataReader = dataReader;
            Handler = (ValueNodeEventArgs args) =>
            {
                switch (args.Path)
                {
                    case "/ID_LINE":
                        args.Value = _dataReader["ID_LINE"];
                        break;
                    case "/ID_ROUTE":
                        args.Value = _dataReader["ID_ROUTE"];
                        break;
                    case "/ActualArrival":
                        args.Value = _dataReader["ActualArrival"];
                        break;
                    case "/ActualDeparture":
                        args.Value = _dataReader["ActualDeparture"];
                        break;
                    case "/AdditionalInfo":
                        args.Value = _dataReader["AdditionalInfo"];
                        break;
                    case "/Condition":
                        args.Value = _dataReader["Condition"];
                        break;
                    case "/ScheduledArrival":
                        args.Value = _dataReader["ScheduledArrival"];
                        break;
                    case "/ScheduledDeparture":
                        args.Value = _dataReader["ScheduledDeparture"];
                        break;
                    case "/Voyage":
                        args.Value = _dataReader["Voyage"];
                        break;
                    case "/VoyageAlt":
                        args.Value = _dataReader["VoyageAlt"];
                        break;
                    case "/Location/ID_LOCATION":
                        args.Value = _dataReader["Location.ID_LOCATION"];
                        break;
                    case "/Location/Name":
                        args.Value = _dataReader["Location.Name"];
                        break;
                    case "/Location/ShortName":
                        args.Value = _dataReader["Location.ShortName"];
                        break;
                    case "/Location/Type":
                        args.Value = _dataReader["Location.Type"];
                        break;
                    case "/Location/Unlocode":
                        args.Value = _dataReader["Location.Unlocode"];
                        break;
                    case "/PrevCall":
                        if (_dataReader["PrevCall.ID_LINE"] is null)
                        {
                            args.Value = null;
                        }
                        break;

                    case "/PrevCall/ID_LINE":
                        args.Value = _dataReader["PrevCall.ID_LINE"];
                        break;
                    case "/PrevCall/ID_ROUTE":
                        args.Value = _dataReader["PrevCall.ID_ROUTE"];
                        break;
                    case "/PrevCall/ActualArrival":
                        args.Value = _dataReader["PrevCall.ActualArrival"];
                        break;
                    case "/PrevCall/ActualDeparture":
                        args.Value = _dataReader["PrevCall.ActualDeparture"];
                        break;
                    case "/PrevCall/AdditionalInfo":
                        args.Value = _dataReader["PrevCall.AdditionalInfo"];
                        break;
                    case "/PrevCall/Condition":
                        args.Value = _dataReader["PrevCall.Condition"];
                        break;
                    case "/PrevCall/ScheduledArrival":
                        args.Value = _dataReader["PrevCall.ScheduledArrival"];
                        break;
                    case "/PrevCall/ScheduledDeparture":
                        args.Value = _dataReader["PrevCall.ScheduledDeparture"];
                        break;
                    case "/PrevCall/Voyage":
                        args.Value = _dataReader["PrevCall.Voyage"];
                        break;
                    case "/PrevCall/VoyageAlt":
                        args.Value = _dataReader["PrevCall.VoyageAlt"];
                        break;
                    case "/PrevCall/Location/ID_LOCATION":
                        args.Value = _dataReader["PrevCall.Location.ID_LOCATION"];
                        break;
                    case "/PrevCall/Location/Name":
                        args.Value = _dataReader["PrevCall.Location.Name"];
                        break;
                    case "/PrevCall/Location/ShortName":
                        args.Value = _dataReader["PrevCall.Location.ShortName"];
                        break;
                    case "/PrevCall/Location/Type":
                        args.Value = _dataReader["PrevCall.Location.Type"];
                        break;
                    case "/PrevCall/Location/Unlocode":
                        args.Value = _dataReader["PrevCall.Location.Unlocode"];
                        break;
                    case "/PrevCall/Route/ID_LINE":
                        args.Value = _dataReader["PrevCall.Route.ID_LINE"];
                        break;
                    case "/PrevCall/Route/ID_RHEAD":
                        args.Value = _dataReader["PrevCall.Route.ID_RHEAD"];
                        break;
                    case "/PrevCall/Route/Line/ID_LINE":
                        args.Value = _dataReader["PrevCall.Route.Line.ID_LINE"];
                        break;
                    case "/PrevCall/Route/Line/Description":
                        args.Value = _dataReader["PrevCall.Route.Line.Description"];
                        break;
                    case "/PrevCall/Route/Line/Name":
                        args.Value = _dataReader["PrevCall.Route.Line.Name"];
                        break;
                    case "/PrevCall/Route/Line/ShortName":
                        args.Value = _dataReader["PrevCall.Route.Line.ShortName"];
                        break;
                    case "/PrevCall/Route/Vessel/ID_VESSEL":
                        args.Value = _dataReader["PrevCall.Route.Vessel.ID_VESSEL"];
                        break;
                    case "/PrevCall/Route/Vessel/Brutto":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Brutto"];
                        break;
                    case "/PrevCall/Route/Vessel/CallSign":
                        args.Value = _dataReader["PrevCall.Route.Vessel.CallSign"];
                        break;
                    case "/PrevCall/Route/Vessel/Description":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Description"];
                        break;
                    case "/PrevCall/Route/Vessel/Height":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Height"];
                        break;
                    case "/PrevCall/Route/Vessel/IsOcean":
                        args.Value = _dataReader["PrevCall.Route.Vessel.IsOcean"];
                        break;
                    case "/PrevCall/Route/Vessel/Length":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Length"];
                        break;
                    case "/PrevCall/Route/Vessel/LineMeters":
                        args.Value = _dataReader["PrevCall.Route.Vessel.LineMeters"];
                        break;
                    case "/PrevCall/Route/Vessel/Name":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Name"];
                        break;
                    case "/PrevCall/Route/Vessel/Netto":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Netto"];
                        break;
                    case "/PrevCall/Route/Vessel/RiffCount":
                        args.Value = _dataReader["PrevCall.Route.Vessel.RiffCount"];
                        break;
                    case "/PrevCall/Route/Vessel/Width":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Width"];
                        break;
                    case "/PrevCall/Route/Vessel/Port":
                        if (_dataReader["PrevCall.Route.Vessel.Port.ID_LOCATION"] is null)
                        {
                            args.Value = null;
                        }
                        break;
                    case "/PrevCall/Route/Vessel/Port/ID_LOCATION":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Port.ID_LOCATION"];
                        break;
                    case "/PrevCall/Route/Vessel/Port/Name":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Port.Name"];
                        break;
                    case "/PrevCall/Route/Vessel/Port/ShortName":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Port.ShortName"];
                        break;
                    case "/PrevCall/Route/Vessel/Port/Type":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Port.Type"];
                        break;
                    case "/PrevCall/Route/Vessel/Port/Unlocode":
                        args.Value = _dataReader["PrevCall.Route.Vessel.Port.Unlocode"];
                        break;
                    case "/Route/ID_LINE":
                        args.Value = _dataReader["Route.ID_LINE"];
                        break;
                    case "/Route/ID_RHEAD":
                        args.Value = _dataReader["Route.ID_RHEAD"];
                        break;
                    case "/Route/Line/ID_LINE":
                        args.Value = _dataReader["Route.Line.ID_LINE"];
                        break;
                    case "/Route/Line/Description":
                        args.Value = _dataReader["Route.Line.Description"];
                        break;
                    case "/Route/Line/Name":
                        args.Value = _dataReader["Route.Line.Name"];
                        break;
                    case "/Route/Line/ShortName":
                        args.Value = _dataReader["Route.Line.ShortName"];
                        break;
                    case "/Route/Vessel/ID_VESSEL":
                        args.Value = _dataReader["Route.Vessel.ID_VESSEL"];
                        break;
                    case "/Route/Vessel/Brutto":
                        args.Value = _dataReader["Route.Vessel.Brutto"];
                        break;
                    case "/Route/Vessel/CallSign":
                        args.Value = _dataReader["Route.Vessel.CallSign"];
                        break;
                    case "/Route/Vessel/Description":
                        args.Value = _dataReader["Route.Vessel.Description"];
                        break;
                    case "/Route/Vessel/Height":
                        args.Value = _dataReader["Route.Vessel.Height"];
                        break;
                    case "/Route/Vessel/IsOcean":
                        args.Value = _dataReader["Route.Vessel.IsOcean"];
                        break;
                    case "/Route/Vessel/Length":
                        args.Value = _dataReader["Route.Vessel.Length"];
                        break;
                    case "/Route/Vessel/LineMeters":
                        args.Value = _dataReader["Route.Vessel.LineMeters"];
                        break;
                    case "/Route/Vessel/Name":
                        args.Value = _dataReader["Route.Vessel.Name"];
                        break;
                    case "/Route/Vessel/Netto":
                        args.Value = _dataReader["Route.Vessel.Netto"];
                        break;
                    case "/Route/Vessel/RiffCount":
                        args.Value = _dataReader["Route.Vessel.RiffCount"];
                        break;
                    case "/Route/Vessel/Width":
                        args.Value = _dataReader["Route.Vessel.Width"];
                        break;
                    case "/Route/Vessel/Port":
                        if (_dataReader["Route.Vessel.Port.ID_LOCATION"] is null)
                        {
                            args.Value = null;
                        }
                        break;
                    case "/Route/Vessel/Port/ID_LOCATION":
                        args.Value = _dataReader["Route.Vessel.Port.ID_LOCATION"];
                        break;
                    case "/Route/Vessel/Port/Name":
                        args.Value = _dataReader["Route.Vessel.Port.Name"];
                        break;
                    case "/Route/Vessel/Port/ShortName":
                        args.Value = _dataReader["Route.Vessel.Port.ShortName"];
                        break;
                    case "/Route/Vessel/Port/Type":
                        args.Value = _dataReader["Route.Vessel.Port.Type"];
                        break;
                    case "/Route/Vessel/Port/Unlocode":
                        args.Value = _dataReader["Route.Vessel.Port.Unlocode"];
                        break;
                }
            };
        }

    }

}
