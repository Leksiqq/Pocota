namespace Net.Leksi.Pocota.Client;

public class PocotaContext
{
    private readonly IServiceProvider _services;
    protected ulong _idGen = 0;
    public PocotaContext(IServiceProvider services)
    {
        _services = services;
    }
}
