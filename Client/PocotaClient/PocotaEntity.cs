

using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Client;

public abstract class PocotaEntity: IPocotaEntity
{
    private AccessKind _access = AccessKind.Full;
    protected readonly PocotaContext _context;
    public ulong PocotaId {  get; private init; }
    public EntityState State { get; internal set; } = EntityState.Detached;
    public AccessKind Access
    {
        get => _access;
        internal set
        {
            if (_access != value)
            {
                if (value is AccessKind.Full || value is AccessKind.Readonly || value is AccessKind.Anonym)
                {
                    _access = value;
                }
                else
                {
                    _access = AccessKind.Readonly;
                }
            }
        }
    }
    public IEnumerable<EntityProperty> Properties => GetProperties();
    public PocotaEntity(ulong pocotaId, PocotaContext context)
    {
        PocotaId = pocotaId;
        _context = context;
    }
    protected abstract IEnumerable<EntityProperty> GetProperties();
}
