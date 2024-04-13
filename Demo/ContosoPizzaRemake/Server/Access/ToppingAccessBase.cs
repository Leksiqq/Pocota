/////////////////////////////////////////////////////////////
// ContosoPizza.Models.ToppingAccessBase                   //
// was generated automatically from ContosoPizza.IContract //
// at 2024-04-13T13:50:47.                                 //
// Modifying this file will break the program!             //
/////////////////////////////////////////////////////////////

using Net.Leksi.Pocota.Server;

namespace ContosoPizza.Models;


public class ToppingAccessBase
{
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    public ToppingAccessBase(IServiceProvider services)
    {
        _services = services;
        _context = _services.GetRequiredService<PocotaContext>();
    }
}