using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota;
using Net.Leksi.Pocota.Core;
using PocotaTestProject.Model;
using System.Collections;
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

        IHost host1 = Config.Configure1();
        List<ShipCall> shipCalls = new();
        for (int i = 0; i < shipCallsCount; ++i)
        {
            shipCalls.Add(host1.Services.GetRequiredService<ShipCall>());
        }
        Dictionary<Type, HashSet<int>> objectsCounts = CountObjects(host1.Services, shipCalls);

        Assert.That(objectsCounts[typeof(ShipCall)].Count, Is.EqualTo(shipCallsCount * 2));
        Assert.That(objectsCounts[typeof(Location)].Count, Is.EqualTo(shipCallsCount * 2));
        Assert.That(objectsCounts[typeof(Line)].Count, Is.EqualTo(shipCallsCount));
        Assert.That(objectsCounts[typeof(Vessel)].Count, Is.EqualTo(shipCallsCount));
        Assert.That(objectsCounts[typeof(Route)].Count, Is.EqualTo(shipCallsCount));

        List<ShipCall> shipCalls1 = new();

        DbDataReader dataReader = new ModelDataReader(shipCalls, host1.Services);
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
                    if (verbose)
                    {
                        Trace.WriteLine($"{obj.GetType()}: {obj.GetHashCode()} {string.Join(", ", container.GetKeyRing(obj).Select(v => v.Key + ":" + v.Value))}");
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
        private readonly List<ShipCall> _shipCalls;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, KeyRing?> keys = new();
        private int _position = -1;
        private readonly Container _container;

        internal ModelDataReader(List<ShipCall> shipCalls, IServiceProvider serviceProvider) => 
            (_shipCalls, _serviceProvider, _container) = (shipCalls, serviceProvider, serviceProvider.GetRequiredService<Container>());

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
            if(++_position < _shipCalls.Count)
            {
                keys["/"] = _container.GetKeyRing(_shipCalls[_position]);
                keys["/Location"] = _container.GetKeyRing(_shipCalls[_position].Location);
                keys["/PrevCall"] = _shipCalls[_position].PrevCall is { } ? _container.GetKeyRing(_shipCalls[_position].Location) : null;
                keys["/Route"] = _container.GetKeyRing(_shipCalls[_position].Route);
                keys["/Route/Line"] = _container.GetKeyRing(_shipCalls[_position].Route.Line);
                keys["/Route/Vessel"] = _container.GetKeyRing(_shipCalls[_position].Route.Vessel);
                keys["/Route/Vessel/Port"] = _container.GetKeyRing(_shipCalls[_position].Route.Vessel.Port);
                return true;
            }
            return false;
        }
    }

}
