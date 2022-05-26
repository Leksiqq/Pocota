namespace PocotaTestProject.Model;

public interface ITravelForListing
{
    IDepartureShipCall DepartureShipCall { get; }
    IArrivalShipCall? ArrivalShipCall { get; }
}
