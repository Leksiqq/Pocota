using System;

namespace PocotaTestProject.Model;

public interface IShipCallForListing: IShipCallAdditionalInfo
{
    IRouteShort Route { get; }
    string Voyage { get;}
    string VoyageAlt { get; }
    ILocation Location { get; }
    DateTime ScheduledArrival { get; }
    DateTime ActualArrival { get; }
    DateTime ScheduledDeparture { get; }
    DateTime ActualDeparture { get; }
    ShipCallCondition Condition { get; }
}
