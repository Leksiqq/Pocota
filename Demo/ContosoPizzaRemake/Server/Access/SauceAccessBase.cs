/////////////////////////////////////////////////////////////
// ContosoPizza.Models.SauceAccessBase                     //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-14T15:28:52.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Server;

namespace ContosoPizza.Models;


public class SauceAccessBase
{
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    public SauceAccessBase(IServiceProvider services)
    {
        _services = services;
        _context = _services.GetRequiredService<PocotaContext>();
    }
}