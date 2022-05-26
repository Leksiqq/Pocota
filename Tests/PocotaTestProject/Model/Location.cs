namespace PocotaTestProject.Model;

public class Location : ILocation
{
    public LocationType Type {get; set;}

    public string ShortName { get; set; }

    public string Name { get; set; }

    public string Unlocode { get; set; }
}
