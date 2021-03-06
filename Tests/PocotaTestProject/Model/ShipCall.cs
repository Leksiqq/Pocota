using System;

namespace PocotaTestProject.Model;

public class ShipCall : IShipCall, IShipCallForListing, IDepartureShipCall, IArrivalShipCall
{
    public Route Route { get; set; }

    public string Voyage { get; set; }

    public string VoyageAlt { get; set; }

    public Location Location { get; set; }

    public DateTime ScheduledArrival { get; set; }

    public DateTime ActualArrival { get; set; }

    public DateTime ScheduledDeparture { get; set; }

    public DateTime ActualDeparture { get; set; }

    public ShipCallCondition Condition { get; set; }

    public string AdditionalInfo { get; set; } = "N/A";

    public ShipCall? PrevCall { get; set; }

    IRoute IShipCall.Route => Route;

    IRouteShort IShipCallForListing.Route => Route;

    IRouteShort IDepartureShipCall.Route => Route;

    ILocation IShipCall.Location => Location;

    ILocation IShipCallForListing.Location => Location;

    ILocation IDepartureShipCall.Location => Location;

    ILocation IArrivalShipCall.Location => Location;

    IShipCall? IShipCall.PrevCall => PrevCall;
}
