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
            });
        return hostBuilder.Build();
    }

    public static IHost Configure1()
    {
        IHostBuilder hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .AddPocotaCore(services =>
            {
                services.AddTransient<ShipCall>(op => GetShipCall(op.GetRequiredService<Container>()));
                services.AddTransient<IShipCall, ShipCall>(op => GetShipCall(op.GetRequiredService<Container>()));
                services.AddTransient<IShipCallForListing, ShipCall>(op => GetShipCall(op.GetRequiredService<Container>()));
                services.AddTransient<IShipCallAdditionalInfo, ShipCall>(op => GetShipCall(op.GetRequiredService<Container>()));
                services.AddTransient<IArrivalShipCall, ShipCall>(op => GetShipCall(op.GetRequiredService<Container>()));
                services.AddTransient<IDepartureShipCall, ShipCall>(op => GetShipCall(op.GetRequiredService<Container>()));
                services.AddPrimaryKey<ShipCall>(new Dictionary<string, Type> { { "ID_LINE", typeof(string) }, { "ID_ROUTE", typeof(int) } });

                services.AddTransient<Location>(op => GetLocation(op.GetRequiredService<Container>()));
                services.AddTransient<ILocation, Location>(op => GetLocation(op.GetRequiredService<Container>()));
                services.AddPrimaryKey<Location>(new Dictionary<string, Type> { { "ID_LOCATION", typeof(string) } });

                services.AddTransient<Route>(op => GetRoute(op.GetRequiredService<Container>()));
                services.AddTransient<IRoute, Route>(op => GetRoute(op.GetRequiredService<Container>()));
                services.AddTransient<IRouteShort, Route>(op => GetRoute(op.GetRequiredService<Container>()));
                services.AddPrimaryKey<Route>(new Dictionary<string, Type> { { "ID_LINE", typeof(string) }, { "ID_RHEAD", typeof(int) } });

                services.AddTransient<Line>(op => GetLine(op.GetRequiredService<Container>()));
                services.AddTransient<ILine, Line>(op => GetLine(op.GetRequiredService<Container>()));
                services.AddPrimaryKey<Line>(new Dictionary<string, Type> { { "ID_LINE", typeof(string) } });

                services.AddTransient<Vessel>(op => GetVessel(op.GetRequiredService<Container>()));
                services.AddTransient<IVessel, Vessel>(op => GetVessel(op.GetRequiredService<Container>()));
                services.AddTransient<IVesselShort, Vessel>(op => GetVessel(op.GetRequiredService<Container>()));
                services.AddPrimaryKey<Vessel>(new Dictionary<string, Type> { { "ID_VESSEL", typeof(string) } });

                services.AddTransient<Travel>(op => GetTravel(op.GetRequiredService<Container>()));
                services.AddTransient<ITravelForListing, Travel>(op => GetTravel(op.GetRequiredService<Container>()));
            }).ConfigureServices(services =>
            {
                services.AddTransient<PocoBuilder>();
            });
        return hostBuilder.Build();
    }

    private static Travel GetTravel(Container container)
    {
        Travel result = new();

        result.ArrivalShipCall = GetShipCall(container);
        result.DepartureShipCall = GetShipCall(container);

        return result;
    }

    private static Line GetLine(Container container)
    {
        Line result = new();
        result.ShortName = "TRE";
        result.Name = "TransRussiaExpress";
        container.GetKeyRing(result)!.SetField("ID_LINE", "TRE");
        return result;
    }

    private static Route GetRoute(Container container)
    {
        Route result = new();
        result.Line = GetLine(container);
        result.Vessel = GetVessel(container);
        container.GetKeyRing(result)!.SetField("ID_RHEAD", 1).SetField("ID_LINE", container.GetKeyRing(result.Line)!["ID_LINE"]);
        return result;
    }

    private static Vessel GetVessel(Container container)
    {
        Vessel result = new();
        container.GetKeyRing(result)!.SetField("ID_VESSEL", "FINNSUN");
        result.LineMeters = 1500;
        result.Brutto = 28008;
        result.Netto = 8400;
        result.Description = "Some RoRo ship";
        result.CallSign = "OJPA";
        result.IsOcean = false;
        result.Height = 17.15;
        result.Length = 188.38;
        result.Name = "MV \"Finnsun\"";
        result.Port = GetLocation(container);
        result.RiffCount = 15;
        result.Width = 26.51;
        return result;
    }

    private static string[] locations = new[] { "HELSINKI", "LUEBECK", "SPB-BRONKA", "TRAVEMUNDE", "ANTWERPEN" };
    private static int nextLocation = 0;

    private static Location GetLocation(Container container)
    {
        Location result = new();
        KeyRing? keyRing = container.GetKeyRing(result);
        keyRing!["ID_LOCATION"] = locations[(nextLocation++) % locations.Length];
        result.ShortName = keyRing!["ID_LOCATION"].ToString()!;
        result.Name = result.ShortName.Substring(0, 1) + result.ShortName.ToLower().Substring(1);
        result.Type = LocationType.Port;
        result.Unlocode = String.Empty;
        return result;
    }

    private static int nextIdShipCall = 1;

    private static ShipCall GetShipCall(Container container, int id = -1)
    {
        ShipCall result = new();
        if (Environment.StackTrace.Split("\n", StringSplitOptions.RemoveEmptyEntries).Where(s => s.Contains("GetShipCall")).Count() == 1)
        {
            result.PrevCall = GetShipCall(container, nextIdShipCall);
        }
        result.Route = GetRoute(container);
        container.GetKeyRing(result)!.SetField("ID_ROUTE", id == -1 ? ++nextIdShipCall : id).SetField("ID_LINE", container.GetKeyRing(result.Route.Line)!["ID_LINE"]);
        result.ActualArrival = DateTime.Parse("2021-12-31T08:00:00");
        result.ActualDeparture = DateTime.Parse("2021-12-31T13:00:00");
        result.Condition = ShipCallCondition.Closed;
        result.Location = GetLocation(container)!;
        result.ScheduledArrival = result.ActualArrival;
        result.ScheduledDeparture = result.ActualDeparture;
        result.Voyage = "FIU22001";
        result.VoyageAlt = "UNASSIGNED";
        return result;
    }
}
