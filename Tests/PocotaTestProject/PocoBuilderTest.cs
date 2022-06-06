using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota;
using Net.Leksi.Pocota.Core;
using PocotaTestProject.Model;
using System.Collections;
using System.Data;
using System.Data.Common;
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
        Config.ModelObjectFactory modelObjectFactory = _host.Services.GetRequiredService<Config.ModelObjectFactory>();

        List<ShipCall> shipCalls = new();
        for (int i = 0; i < shipCallsCount; ++i)
        {
            shipCalls.Add(modelObjectFactory.Create<ShipCall>());
        }
        Dictionary<Type, HashSet<int>> objectsCounts = CountObjects(_host.Services, shipCalls);

        Assert.That(objectsCounts[typeof(ShipCall)].Count, Is.EqualTo(shipCallsCount * 2));
        Assert.That(objectsCounts[typeof(Location)].Count, Is.EqualTo(shipCallsCount * 4));
        Assert.That(objectsCounts[typeof(Line)].Count, Is.EqualTo(shipCallsCount * 2));
        Assert.That(objectsCounts[typeof(Vessel)].Count, Is.EqualTo(shipCallsCount * 2));
        Assert.That(objectsCounts[typeof(Route)].Count, Is.EqualTo(shipCallsCount * 2));

        List<ShipCall> shipCalls1 = new();

        DbDataReader dataReader = new ModelDataReader(shipCalls, _host.Services);
        CallShipBuildHandler buildHandler = new(dataReader);
        PocoBuilder builder = _host.Services.GetRequiredService<PocoBuilder>();


        while (dataReader.Read())
        {
            shipCalls1.Add(builder.Build<ShipCall>(buildHandler.Handler));
        }

        objectsCounts = CountObjects(_host.Services, shipCalls1, true);

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

            foreach(var obj in objects[typeof(ShipCall)])
            {
                Trace.WriteLine(typesForest.TreeToString(obj));
            }
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

    internal class ModelDataReader : DbDataReader
    {
        private readonly DataSet _data = new();
        private int _position = -1;
        private DataRow _shipCallRow;
        private DataRow _lineRow;
        private DataRow _routeRow;
        private DataRow _vesselRow;
        private DataRow _shipCallPortRow;
        private DataRow _vesselPortRow;
        private DataRow _prevShipCallRow;
        private DataRow _prevShipCallPortRow;


        internal ModelDataReader()
        {
            _data.Tables.Add("Lines");
            _data.Tables["Lines"]!.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID_LINE", typeof(string)),
                new DataColumn("ShortName", typeof(string)),
                new DataColumn("Name", typeof(string)),
            });
            _data.Tables["Lines"]!.Rows.Add(new object[] { "TRE", "TRE", "TransRussiaExpress" });

            _data.Tables.Add("Vessels");
            _data.Tables["Vessels"]!.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID_VESSEL", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Length", typeof(double)),
                new DataColumn("Width", typeof(double)),
                new DataColumn("Height", typeof(double)),
                new DataColumn("Brutto", typeof(double)),
                new DataColumn("Netto", typeof(double)),
                new DataColumn("LineMeters", typeof(double)),
                new DataColumn("Description", typeof(string)),
                new DataColumn("RiffCount", typeof(int)),
                new DataColumn("IsOcean", typeof(bool)),
                new DataColumn("CallSign", typeof(string)),
                new DataColumn("ID_PORT", typeof(string)),
            });
            _data.Tables["Vessels"]!.Rows.Add(new object[] { "FINNSUN", "MV \"Finnsun\"", 188.38, 26.51, 17.15, 28008, 8400, 1500, "RoRo ship", 15, false, "OJPA", "HELSINKI" });

            _data.Tables.Add("Locations");
            _data.Tables["Locations"]!.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID_LOCATION", typeof(string)),
                new DataColumn("ShortName", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Type", typeof(LocationType)),
                new DataColumn("Unlocode", typeof(string)),
            });
            _data.Tables["Locations"]!.Rows.Add(new object[] { "HELSINKI", "Helsinki", "Helsinki", LocationType.Port, "FIHEL" });
            _data.Tables["Locations"]!.Rows.Add(new object[] { "LUEBECK", "Luebeck", "Luebeck", LocationType.Port, "DELBC" });
            _data.Tables["Locations"]!.Rows.Add(new object[] { "SPB-BRONKA", "Bronka", "Bronka, SPB", LocationType.Port, "RULED" });
            _data.Tables["Locations"]!.Rows.Add(new object[] { "STOCKHOLM", "Stockholm", "Stockholm", LocationType.Port, "SESTO" });
            _data.Tables["Locations"]!.Rows.Add(new object[] { "ANTWERPEN", "Antwerpen", "Antwerpen", LocationType.Port, "BEANR" });

            _data.Tables.Add("Routes");
            _data.Tables["Routes"]!.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID_RHEAD", typeof(int)),
                new DataColumn("ID_LINE", typeof(string)),
                new DataColumn("ID_VESSEL", typeof(string)),
            });
            _data.Tables["Lines"]!.Rows.Add(new object[] { 1, "TRE", "FINNSUN" });

            _data.Tables.Add("Routes");
            _data.Tables["Routes"]!.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID_RHEAD", typeof(int)),
                new DataColumn("ID_LINE", typeof(string)),
                new DataColumn("ID_VESSEL", typeof(string)),
            });
            _data.Tables["Lines"]!.Rows.Add(new object[] { 1, "TRE", "FINNSUN" });

            _data.Tables.Add("ShipCalls");
            _data.Tables["ShipCalls"]!.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID_ROUTE", typeof(int)),
                new DataColumn("ID_RHEAD", typeof(int)),
                new DataColumn("ID_LINE", typeof(string)),
                new DataColumn("Voyage", typeof(string)),
                new DataColumn("VoyageAlt", typeof(string)),
                new DataColumn("ID_PORT", typeof(string)),
                new DataColumn("ScheduledArrival", typeof(DateTime)),
                new DataColumn("ActualArrival", typeof(DateTime)),
                new DataColumn("ScheduledDeparture", typeof(DateTime)),
                new DataColumn("ActualDeparture", typeof(DateTime)),
                new DataColumn("Condition", typeof(ShipCallCondition)),
                new DataColumn("AdditionalInfo", typeof(string)),
                new DataColumn("ID_ROUTE_PREV", typeof(int)),
            });
            _data.Tables["ShipCalls"]!.Rows.Add(new object[] {
                2, 1, "TRE", "FIS22001", string.Empty, "ANTWERPEN",
                DateTime.Parse("2021-01-01T06:00:00"), DateTime.Parse("2021-01-01T06:23:00"),
                DateTime.Parse("2021-01-01T13:00:00"), DateTime.Parse("2021-01-01T13:12:00"),
                ShipCallCondition.Closed, "c:2032(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:42(42)  D:0", 1
            });
            _data.Tables["ShipCalls"]!.Rows.Add(new object[] {
                3, 1, "TRE", "FIS22001", string.Empty, "LUEBECK",
                DateTime.Parse("2021-01-02T06:00:00"), DateTime.Parse("2021-01-02T06:23:00"),
                DateTime.Parse("2021-01-02T13:00:00"), DateTime.Parse("2021-01-02T13:12:00"),
                ShipCallCondition.Closed, "c:740(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:29(29)  D:0", 2
            });
            _data.Tables["ShipCalls"]!.Rows.Add(new object[] {
                4, 1, "TRE", "FIS22001", string.Empty, "STOCKHOLM",
                DateTime.Parse("2021-01-03T06:00:00"), DateTime.Parse("2021-01-03T06:23:00"),
                DateTime.Parse("2021-01-03T13:00:00"), DateTime.Parse("2021-01-03T13:12:00"),
                ShipCallCondition.Closed, "c:2105(2105)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:50(50)  D:0", 3
            });
            _data.Tables["ShipCalls"]!.Rows.Add(new object[] {
                5, 1, "TRE", "FIS22001", string.Empty, "HELSINKI",
                DateTime.Parse("2021-01-04T06:00:00"), DateTime.Parse("2021-01-04T06:23:00"),
                DateTime.Parse("2021-01-04T13:00:00"), DateTime.Parse("2021-01-04T13:12:00"),
                ShipCallCondition.Closed, "c:519(519)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:4(4)  D:0", 4
            });
            _data.Tables["ShipCalls"]!.Rows.Add(new object[] {
                6, 1, "TRE", "FIS22002", string.Empty, "SPB-BRONKA",
                DateTime.Parse("2021-01-05T06:00:00"), DateTime.Parse("2021-01-05T06:23:00"),
                DateTime.Parse("2021-01-05T13:00:00"), DateTime.Parse("2021-01-05T13:12:00"),
                ShipCallCondition.Closed, "c:1406(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:34(34)  D:0", 5
            });
            _data.Tables["ShipCalls"]!.Rows.Add(new object[] {
                7, 1, "TRE", "FIS22002", string.Empty, "HELSINKI",
                DateTime.Parse("2021-01-06T06:00:00"), DateTime.Parse("2021-01-06T06:23:00"),
                DateTime.Parse("2021-01-06T13:00:00"), DateTime.Parse("2021-01-06T13:12:00"),
                ShipCallCondition.Closed, "c:834(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:29(29)  D:0", 6
            });
            _data.Tables["ShipCalls"]!.Rows.Add(new object[] {
                8, 1, "TRE", "FIS22002", string.Empty, "STOCKHOLM",
                DateTime.Parse("2021-01-07T06:00:00"), DateTime.Parse("2021-01-07T06:23:00"),
                DateTime.Parse("2021-01-07T13:00:00"), DateTime.Parse("2021-01-07T13:12:00"),
                ShipCallCondition.Closed, "c:2356(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:46(48)  D:0", 7
            });
            _data.Tables["ShipCalls"]!.Rows.Add(new object[] {
                9, 1, "TRE", "FIS22002", string.Empty, "LUEBECK",
                DateTime.Parse("2021-01-08T06:00:00"), DateTime.Parse("2021-01-08T06:23:00"),
                DateTime.Parse("2021-01-08T13:00:00"), DateTime.Parse("2021-01-08T13:12:00"),
                ShipCallCondition.Closed, "c:345(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:54(54)  D:0", 8
            });
            _data.Tables["ShipCalls"]!.Rows.Add(new object[] {
                10, 1, "TRE", "FIS22003", string.Empty, "ANTWERPEN",
                DateTime.Parse("2021-01-09T06:00:00"), DateTime.Parse("2021-01-09T06:23:00"),
                DateTime.Parse("2021-01-09T13:00:00"), DateTime.Parse("2021-01-09T13:12:00"),
                ShipCallCondition.Closed, "c:765(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:34(34)  D:0", 9
            });
            _data.Tables["ShipCalls"]!.Rows.Add(new object[] {
                10, 1, "TRE", "FIS22003", string.Empty, "LUEBECK",
                DateTime.Parse("2021-01-10T06:00:00"), DateTime.Parse("2021-01-10T06:23:00"),
                DateTime.Parse("2021-01-10T13:00:00"), DateTime.Parse("2021-01-10T13:12:00"),
                ShipCallCondition.Closed, "c:284(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:67(67)  D:0", 10
            });


        }

        public void Open()
        {
            _position = -1;
        }

        public override object this[int ordinal] => throw new NotImplementedException();

        public override object this[string name] { 
            get 
            {
                string path = $"/{name.Replace('.', '/')}";
                KeyValuePair<string, KeyRing?> keyRingPair = keys.Where(e => path.StartsWith(e.Key))
                    .Select(e => new KeyValuePair<string, KeyRing?>(path.Substring($"{e.Key}/".Replace("//", "/").Length), e.Value))
                    .Where(e => !e.Key.Contains("/") && (e.Value is null || e.Value.ContainsKey(e.Key))).FirstOrDefault();
                if(keyRingPair is { } && keyRingPair.Key is { })
                {
                    return keyRingPair.Value?[keyRingPair.Key] ?? null;
                }
                else
                {
                    string[] properties = name.Split('.');
                    object current = _shipCalls[_position];
                    for(int i = 0; i < properties.Length && current is { }; ++i)
                    {
                        current = current.GetType().GetProperty(properties[i])?.GetValue(current) ?? null;
                    }
                    return current;
                }
            } 
        }

        #region unused
        public override int Depth => throw new NotImplementedException();

        public override int FieldCount => throw new NotImplementedException();

        public override bool HasRows => throw new NotImplementedException();

        public override bool IsClosed => throw new NotImplementedException();

        public override int RecordsAffected => throw new NotImplementedException();

        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetInt32(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override bool NextResult()
        {
            throw new NotImplementedException();
        }
        #endregion unused
        public override bool Read()
        {
            if(++_position < _data.Tables["ShipCalls"]!.Rows.Count)
            {
                _shipCallRow = _data.Tables["ShipCalls"]!.Rows[_position];
                _lineRow = _data.Tables["Lines"]!.Select($"ID_LINE='{_shipCallRow["ID_LINE"]}'").First();
                _routeRow = _data.Tables["Routes"]!.Select($"ID_RHEAD={_shipCallRow["ID_RHEAD"]} and ID_LINE='{_shipCallRow["ID_LINE"]}'").First();
                _vesselRow = _data.Tables["Vessels"]!.Select($"ID_VESSEL='{_routeRow["ID_VESSEL"]}'").First();
                _shipCallPortRow = _data.Tables["Locations"]!.Select($"ID_LOCATION='{_shipCallRow["ID_PORT"]}'").First();
                _vesselPortRow = _data.Tables["Locations"]!.Select($"ID_LOCATION='{_vesselRow["ID_PORT"]}'").First();
                _prevShipCallRow = _data.Tables["ShipCalls"]!.Select($"ID_ROUTE={_shipCallRow["ID_ROUTE_PREV"]} and ID_LINE='{_shipCallRow["ID_LINE"]}'").First();
                _prevShipCallPortRow = _data.Tables["Locations"]!.Select($"ID_LOCATION='{_prevShipCallRow["ID_PORT"]}'").First();
                return true;
            }
            return false;
        }
    }

}
