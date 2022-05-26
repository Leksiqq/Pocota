namespace PocotaTestProject.Model;

public interface IRoute
{
    ILine Line { get; }

    IVessel Vessel { get; }
}
