namespace Net.Leksi.Pocota.Client;

public class PocotaEntity: IPocotaEntity
{
    private readonly ulong _pocotaId;
    protected EntityState _state;
    ulong IPocotaEntity.PocotaId => _pocotaId;
    EntityState IPocotaEntity.State => _state;
    public PocotaEntity(ulong pocotaId)
    {
        _pocotaId = pocotaId;
    }
}
