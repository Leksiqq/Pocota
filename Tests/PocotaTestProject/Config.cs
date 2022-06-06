using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota;
using Net.Leksi.Pocota.Core;
using PocotaTestProject.Model;
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
            });
        return hostBuilder.Build();
    }

    public class ModelObjectFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ModelObjectFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public T? Create<T>() where T : class
        {
            return (T?)Create(typeof(T));
        }

        public object? Create(Type type)
        {
            switch (type.Name)
            {
                case "ShipCall":
                case "IShipCall":
                case "IShipCallForListing":
                case "IShipCallAdditionalInfo":
                case "IArrivalShipCall":
                case "IDepartureShipCall":
                    return GetShipCall(_serviceProvider);
                case "ILocation":
                case "Location":
                    return GetLocation(_serviceProvider);
                case "Route":
                case "IRoute":
                case "IRouteShort":
                    return GetRoute(_serviceProvider);
                case "Line":
                case "ILine":
                    return GetLine(_serviceProvider);
                case "IVessel":
                case "IVesselShort":
                case "Vessel":
                    return GetVessel(_serviceProvider);
                case "Travel":
                case "ITravelForListing":
                    return GetTravel(_serviceProvider);
            }
            return null;
        }

        private Travel GetTravel(IServiceProvider serviceProvider)
        {
            Container container = serviceProvider.GetRequiredService<Container>();
            Travel result = serviceProvider.GetRequiredService<Travel>();

            result.ArrivalShipCall = GetShipCall(serviceProvider);
            result.DepartureShipCall = GetShipCall(serviceProvider);

            return result;
        }

        private Line GetLine(IServiceProvider serviceProvider)
        {
            Container container = serviceProvider.GetRequiredService<Container>();
            Line result = serviceProvider.GetRequiredService<Line>();
            container.CreateKeyRing(result)!.SetField("ID_LINE", "TRE");
            result.ShortName = "TRE";
            result.Name = "TransRussiaExpress";
            return result;
        }

        private Route GetRoute(IServiceProvider serviceProvider)
        {
            Container container = serviceProvider.GetRequiredService<Container>();
            Route result = serviceProvider.GetRequiredService<Route>();
            result.Line = GetLine(serviceProvider);
            result.Vessel = GetVessel(serviceProvider);
            container.GetKeyRing(result)!.SetField("ID_RHEAD", 1).SetField("ID_LINE", container.GetKeyRing(result.Line)!["ID_LINE"]);
            return result;
        }

        private Vessel GetVessel(IServiceProvider serviceProvider)
        {
            Container container = serviceProvider.GetRequiredService<Container>();
            Vessel result = serviceProvider.GetRequiredService<Vessel>();
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
            result.Port = GetLocation(serviceProvider);
            result.RiffCount = 15;
            result.Width = 26.51;
            return result;
        }

        private string[] locations = new[] { "HELSINKI", "LUEBECK", "SPB-BRONKA", "TRAVEMUNDE", "ANTWERPEN" };
        private int nextLocation = 0;

        private Location GetLocation(IServiceProvider serviceProvider)
        {
            Container container = serviceProvider.GetRequiredService<Container>();
            Location result = serviceProvider.GetRequiredService<Location>();
            KeyRing? keyRing = container.GetKeyRing(result);
            keyRing!["ID_LOCATION"] = locations[(nextLocation++) % locations.Length];
            result.ShortName = keyRing!["ID_LOCATION"].ToString()!;
            result.Name = result.ShortName.Substring(0, 1) + result.ShortName.ToLower().Substring(1);
            result.Type = LocationType.Port;
            result.Unlocode = String.Empty;
            return result;
        }

        private int nextIdShipCall = 1;

        private ShipCall GetShipCall(IServiceProvider serviceProvider, int id = -1)
        {
            Container container = serviceProvider.GetRequiredService<Container>();
            ShipCall result = serviceProvider.GetRequiredService<ShipCall>();
            if (Environment.StackTrace.Split("\n", StringSplitOptions.RemoveEmptyEntries).Where(s => s.Contains("GetShipCall")).Count() == 1)
            {
                result.PrevCall = GetShipCall(serviceProvider, nextIdShipCall);
            }
            result.Route = GetRoute(serviceProvider);
            container.GetKeyRing(result)!.SetField("ID_ROUTE", id == -1 ? ++nextIdShipCall : id).SetField("ID_LINE", container.GetKeyRing(result.Route.Line)!["ID_LINE"]);
            result.ActualArrival = DateTime.Parse("2021-12-31T08:00:00");
            result.ActualDeparture = DateTime.Parse("2021-12-31T13:00:00");
            result.Condition = ShipCallCondition.Closed;
            result.Location = GetLocation(serviceProvider)!;
            result.ScheduledArrival = result.ActualArrival;
            result.ScheduledDeparture = result.ActualDeparture;
            result.Voyage = "FIU22001";
            result.VoyageAlt = "UNASSIGNED";
            return result;
        }

    }
}
