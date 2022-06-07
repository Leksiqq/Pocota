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

public class Config
{
    public static IHost Configure()
    {
        IHostBuilder hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .AddPocotaCore(services =>
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
            }).ConfigureServices(services =>
            {
                services.AddTransient<PocoBuilder>();
                services.AddSingleton<ModelObjectFactory>();
                services.AddTransient<ModelDataReader>();
            });
        return hostBuilder.Build();
    }

    public class ModelObjectFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ModelDataReader _dataReader;

        public ModelObjectFactory(IServiceProvider serviceProvider) => 
            (_serviceProvider, _dataReader) = (serviceProvider, serviceProvider.GetRequiredService<ModelDataReader>());

        public ShipCall Create()
        {
            ShipCall shipCall = null;
            if (_dataReader.Read())
            {
                shipCall = _serviceProvider.GetRequiredService<ShipCall>();
                KeyRing key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall)!;
                key["ID_ROUTE"] = _dataReader["ID_ROUTE"];
                key["ID_LINE"] = _dataReader["ID_LINE"];
                shipCall.Location = _serviceProvider.GetRequiredService<Location>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall.Location)!;
                key["ID_LOCATION"] = _dataReader["Location.ID_LOCATION"];
                shipCall.Route = _serviceProvider.GetRequiredService<Route>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall.Route)!;
                key["ID_RHEAD"] = _dataReader["ID_RHEAD"];
                key["ID_LINE"] = _dataReader["ID_LINE"];
                shipCall.Route.Line = _serviceProvider.GetRequiredService<Line>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall.Route.Line)!;
                key["ID_LINE"] = _dataReader["ID_LINE"];
                shipCall.Route.Vessel = _serviceProvider.GetRequiredService<Vessel>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall.Route.Vessel)!;
                key["ID_VESSEL"] = _dataReader["Route.ID_VESSEL"];
                shipCall.Route.Vessel.Port = _serviceProvider.GetRequiredService<Location>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall.Route.Vessel.Port)!;
                key["ID_LOCATION"] = _dataReader["Route.Vessel.Port.ID_LOCATION"];
                shipCall.PrevCall = _serviceProvider.GetRequiredService<ShipCall>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall)!;
                key["ID_ROUTE"] = _dataReader["ID_ROUTE_PREV"];
                key["ID_LINE"] = _dataReader["ID_LINE"];
                shipCall.PrevCall.Location = _serviceProvider.GetRequiredService<Location>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall.Location)!;
                key["ID_LOCATION"] = _dataReader["PrevCall.Location.ID_LOCATION"];
                shipCall.PrevCall.Route = _serviceProvider.GetRequiredService<Route>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall.Route)!;
                key["ID_RHEAD"] = _dataReader["PrevCall.ID_RHEAD"];
                key["ID_LINE"] = _dataReader["PrevCall.ID_LINE"];
                shipCall.PrevCall.Route.Line = _serviceProvider.GetRequiredService<Line>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall.Route.Line)!;
                key["ID_LINE"] = _dataReader["ID_LINE"];
                shipCall.PrevCall.Route.Vessel = _serviceProvider.GetRequiredService<Vessel>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall.Route.Vessel)!;
                key["ID_VESSEL"] = _dataReader["PrevCall.Route.ID_VESSEL"];
                shipCall.PrevCall.Route.Vessel.Port = _serviceProvider.GetRequiredService<Location>();
                key = _serviceProvider.GetRequiredService<Container>().GetKeyRing(shipCall.PrevCall.Route.Vessel.Port)!;
                key["ID_LOCATION"] = _dataReader["PrevCall.Route.Vessel.Port.ID_LOCATION"];
            }
            return shipCall;
        }

    }

    public class ModelDataReader : DbDataReader
    {
        private readonly DataSet _data = new();
        private int _position = 0;
        private Dictionary<string, DataRow?> rows = new();


        public ModelDataReader()
        {
            _data.Tables.Add("Lines");
            _data.Tables["Lines"]!.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID_LINE", typeof(string)),
                new DataColumn("ShortName", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Description", typeof(string)),
            });
            _data.Tables["Lines"]!.Rows.Add("TRE", "TRE", "TransRussiaExpress", "TransRussiaExpress marine line");

            _data.Tables.Add("Locations");
            _data.Tables["Locations"]!.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID_LOCATION", typeof(string)),
                new DataColumn("ShortName", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Type", typeof(LocationType)),
                new DataColumn("Unlocode", typeof(string)),
            });
            _data.Tables["Locations"]!.Rows.Add("HELSINKI", "Helsinki", "Helsinki", LocationType.Port, "FIHEL");
            _data.Tables["Locations"]!.Rows.Add("LUEBECK", "Luebeck", "Luebeck", LocationType.Port, "DELBC");
            _data.Tables["Locations"]!.Rows.Add("SPB-BRONKA", "Bronka", "Bronka, SPB", LocationType.Port, "RULED");
            _data.Tables["Locations"]!.Rows.Add("STOCKHOLM", "Stockholm", "Stockholm", LocationType.Port, "SESTO");
            _data.Tables["Locations"]!.Rows.Add("ANTWERPEN", "Antwerpen", "Antwerpen", LocationType.Port, "BEANR");

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
            _data.Tables["Vessels"]!.Rows.Add("FINNSUN", "MV \"Finnsun\"", 188.38, 26.51, 17.15, 28008, 8400, 1500, "RoRo ship", 15, false, "OJPA", "HELSINKI");


            _data.Tables.Add("Routes");
            _data.Tables["Routes"]!.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID_RHEAD", typeof(int)),
                new DataColumn("ID_LINE", typeof(string)),
                new DataColumn("ID_VESSEL", typeof(string)),
            });
            _data.Tables["Routes"]!.Rows.Add(1, "TRE", "FINNSUN");

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
            _data.Tables["ShipCalls"]!.Rows.Add(
                1, 1, "TRE", "FIS20099", string.Empty, "LUEBECK",
                DateTime.Parse("2020-12-31T06:00:00"), DateTime.Parse("2020-12-31T06:23:00"),
                DateTime.Parse("2020-12-31T13:00:00"), DateTime.Parse("2020-12-31T13:12:00"),
                ShipCallCondition.Closed, "c:2032(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:42(42)  D:0", null
            );
            _data.Tables["ShipCalls"]!.Rows.Add(
                2, 1, "TRE", "FIS21001", string.Empty, "ANTWERPEN",
                DateTime.Parse("2021-01-01T06:00:00"), DateTime.Parse("2021-01-01T06:23:00"),
                DateTime.Parse("2021-01-01T13:00:00"), DateTime.Parse("2021-01-01T13:12:00"),
                ShipCallCondition.Closed, "c:2032(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:42(42)  D:0", 1
            );
            _data.Tables["ShipCalls"]!.Rows.Add(
                3, 1, "TRE", "FIS21001", string.Empty, "LUEBECK",
                DateTime.Parse("2021-01-02T06:00:00"), DateTime.Parse("2021-01-02T06:23:00"),
                DateTime.Parse("2021-01-02T13:00:00"), DateTime.Parse("2021-01-02T13:12:00"),
                ShipCallCondition.Closed, "c:740(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:29(29)  D:0", 2
            );
            _data.Tables["ShipCalls"]!.Rows.Add(
                4, 1, "TRE", "FIS21001", string.Empty, "STOCKHOLM",
                DateTime.Parse("2021-01-03T06:00:00"), DateTime.Parse("2021-01-03T06:23:00"),
                DateTime.Parse("2021-01-03T13:00:00"), DateTime.Parse("2021-01-03T13:12:00"),
                ShipCallCondition.Closed, "c:2105(2105)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:50(50)  D:0", 3
            );
            _data.Tables["ShipCalls"]!.Rows.Add(
                5, 1, "TRE", "FIS21001", string.Empty, "HELSINKI",
                DateTime.Parse("2021-01-04T06:00:00"), DateTime.Parse("2021-01-04T06:23:00"),
                DateTime.Parse("2021-01-04T13:00:00"), DateTime.Parse("2021-01-04T13:12:00"),
                ShipCallCondition.Closed, "c:519(519)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:4(4)  D:0", 4
            );
            _data.Tables["ShipCalls"]!.Rows.Add(
                6, 1, "TRE", "FIS21002", string.Empty, "SPB-BRONKA",
                DateTime.Parse("2021-01-05T06:00:00"), DateTime.Parse("2021-01-05T06:23:00"),
                DateTime.Parse("2021-01-05T13:00:00"), DateTime.Parse("2021-01-05T13:12:00"),
                ShipCallCondition.Closed, "c:1406(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:34(34)  D:0", 5
            );
            _data.Tables["ShipCalls"]!.Rows.Add(
                7, 1, "TRE", "FIS21002", string.Empty, "HELSINKI",
                DateTime.Parse("2021-01-06T06:00:00"), DateTime.Parse("2021-01-06T06:23:00"),
                DateTime.Parse("2021-01-06T13:00:00"), DateTime.Parse("2021-01-06T13:12:00"),
                ShipCallCondition.Closed, "c:834(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:29(29)  D:0", 6
            );
            _data.Tables["ShipCalls"]!.Rows.Add(
                8, 1, "TRE", "FIS21002", string.Empty, "STOCKHOLM",
                DateTime.Parse("2021-01-07T06:00:00"), DateTime.Parse("2021-01-07T06:23:00"),
                DateTime.Parse("2021-01-07T13:00:00"), DateTime.Parse("2021-01-07T13:12:00"),
                ShipCallCondition.Closed, "c:2356(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:46(48)  D:0", 7
            );
            _data.Tables["ShipCalls"]!.Rows.Add(
                9, 1, "TRE", "FIS21002", string.Empty, "LUEBECK",
                DateTime.Parse("2021-01-08T06:00:00"), DateTime.Parse("2021-01-08T06:23:00"),
                DateTime.Parse("2021-01-08T13:00:00"), DateTime.Parse("2021-01-08T13:12:00"),
                ShipCallCondition.Closed, "c:345(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:54(54)  D:0", 8
            );
            _data.Tables["ShipCalls"]!.Rows.Add(
                10, 1, "TRE", "FIS21003", string.Empty, "ANTWERPEN",
                DateTime.Parse("2021-01-09T06:00:00"), DateTime.Parse("2021-01-09T06:23:00"),
                DateTime.Parse("2021-01-09T13:00:00"), DateTime.Parse("2021-01-09T13:12:00"),
                ShipCallCondition.Closed, "c:765(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:34(34)  D:0", 9
            );
            _data.Tables["ShipCalls"]!.Rows.Add(
                11, 1, "TRE", "FIS21003", string.Empty, "LUEBECK",
                DateTime.Parse("2021-01-10T06:00:00"), DateTime.Parse("2021-01-10T06:23:00"),
                DateTime.Parse("2021-01-10T13:00:00"), DateTime.Parse("2021-01-10T13:12:00"),
                ShipCallCondition.Closed, "c:284(0)  b:0(0)   RF=0  c:0(0)  b:0(0)  FT:67(67)  D:0", 10
            );
            _data.AcceptChanges();

        }

        public void Open()
        {
            _position = 0;
        }

        public override object this[int ordinal] => throw new NotImplementedException();

        public override object this[string name]
        {
            get
            {
                string path = $"/{name.Replace('.', '/')}";
                string selector = path.Substring(0, path.LastIndexOf("/"));
                if (string.IsNullOrEmpty(selector))
                {
                    selector = "/";
                }
                DataRow row = rows[selector];
                return row?[path.Substring(path.LastIndexOf("/") + 1)] ?? null;
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
            if (++_position < _data.Tables["ShipCalls"]!.Rows.Count)
            {
                rows["/"] = _data.Tables["ShipCalls"]!.Rows[_position];
                rows["/Location"] = _data.Tables["Locations"]!.Select($"ID_LOCATION='{rows["/"]!["ID_PORT"]}'").FirstOrDefault();
                rows["/PrevCall"] = _data.Tables["ShipCalls"]!.Select($"ID_ROUTE={rows["/"]!["ID_ROUTE_PREV"]} and ID_LINE='{rows["/"]!["ID_LINE"]}'").FirstOrDefault();
                if (rows["/PrevCall"] is { })
                {
                    rows["/PrevCall/Location"] = _data.Tables["Locations"]!.Select($"ID_LOCATION='{rows["/PrevCall"]!["ID_PORT"]}'").FirstOrDefault();
                    rows["/PrevCall/Route"] = _data.Tables["Routes"]!.Select($"ID_RHEAD={rows["/PrevCall"]!["ID_RHEAD"]} and ID_LINE='{rows["/"]!["ID_LINE"]}'").FirstOrDefault();
                    rows["/PrevCall/Route/Line"] = _data.Tables["Lines"]!.Select($"ID_LINE='{rows["/PrevCall/Route"]!["ID_LINE"]}'").FirstOrDefault();
                    rows["/PrevCall/Route/Vessel"] = _data.Tables["Vessels"]!.Select($"ID_VESSEL='{rows["/PrevCall/Route"]!["ID_VESSEL"]}'").FirstOrDefault();
                    rows["/PrevCall/Route/Vessel/Port"] = _data.Tables["Locations"]!.Select($"ID_LOCATION='{rows["/PrevCall/Route/Vessel"]!["ID_PORT"]}'").FirstOrDefault();
                }
                rows["/Route"] = _data.Tables["Routes"]!.Select($"ID_RHEAD={rows["/"]!["ID_RHEAD"]} and ID_LINE='{rows["/"]!["ID_LINE"]}'").FirstOrDefault();
                rows["/Route/Line"] = _data.Tables["Lines"]!.Select($"ID_LINE='{rows["/Route"]!["ID_LINE"]}'").FirstOrDefault();
                rows["/Route/Vessel"] = _data.Tables["Vessels"]!.Select($"ID_VESSEL='{rows["/Route"]!["ID_VESSEL"]}'").FirstOrDefault();
                rows["/Route/Vessel/Port"] = _data.Tables["Locations"]!.Select($"ID_LOCATION='{rows["/Route/Vessel"]!["ID_PORT"]}'").FirstOrDefault();
                return true;
            }
            return false;
        }
    }


}
