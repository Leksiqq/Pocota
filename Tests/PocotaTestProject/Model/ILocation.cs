namespace PocotaTestProject.Model;

public interface ILocation
{
    public LocationType Type { get; }
    public string Unlocode { get; }
    public string Name { get; }
}
