namespace Net.Leksi.Pocota.Client;

public class PocotaEntity: IPocotaEntity
{
    private readonly ulong _pocotaId;
    ulong IPocotaEntity.PocotaId => _pocotaId;
    public PocotaEntity(ulong pocotaId)
    {
        _pocotaId = pocotaId;
    }
}
