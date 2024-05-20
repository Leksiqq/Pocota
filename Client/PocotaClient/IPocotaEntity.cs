namespace Net.Leksi.Pocota.Client;

public interface IPocotaEntity
{
    ulong PocotaId { get; }
    EntityState State { get; }
    IEnumerable<EntityProperty> Properties { get; }
    IPocotaEntity Entity { get; }
}
