namespace Net.Leksi.Pocota.Client;

public class Connector
{
    protected readonly IServiceProvider _services;
    public Connector(IServiceProvider services)
    {
        _services = services;
    }
}
