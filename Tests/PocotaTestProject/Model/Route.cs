namespace PocotaTestProject.Model;

public class Route : IRoute, IRouteShort
{
    public Line Line { get; set; }

    public Vessel Vessel { get; set; }

    ILine IRoute.Line => Line;

    ILine IRouteShort.Line => Line;

    IVessel IRoute.Vessel => Vessel;

    IVesselShort IRouteShort.Vessel => Vessel;
}
