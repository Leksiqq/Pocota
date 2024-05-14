

namespace Net.Leksi.Pocota.Client;

public abstract class PocotaEntity: IPocotaEntity
{
    private readonly ulong _pocotaId;
    protected EntityState _state;
    protected internal PocotaContext _context;
    ulong IPocotaEntity.PocotaId => _pocotaId;
    EntityState IPocotaEntity.State => _state;
    IEnumerable<EntityProperty> IPocotaEntity.Properties => GetProperties();

    public PocotaEntity(ulong pocotaId, PocotaContext context)
    {
        _pocotaId = pocotaId;
        _context = context;
    }
    protected abstract IEnumerable<EntityProperty> GetProperties();
}
