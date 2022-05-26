using System;

namespace PocotaTestProject.Model;

public interface IArrivalShipCall
{
    ILocation Location { get; }
    DateTime ActualArrival { get; }
}
