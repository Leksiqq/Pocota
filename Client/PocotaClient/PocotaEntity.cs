namespace Net.Leksi.Pocota.Client;

public class PocotaEntity: IPocotaEntity
{
    private readonly ulong _pocotaId;
    protected EntityState _state;
    protected internal PocotaContext _context;
    ulong IPocotaEntity.PocotaId => _pocotaId;
    EntityState IPocotaEntity.State => _state;
    public PocotaEntity(ulong pocotaId, PocotaContext context)
    {
        _pocotaId = pocotaId;
        _context = context;
    }
}
