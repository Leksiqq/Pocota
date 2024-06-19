using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Client;

public interface IPocotaEntity
{
    ulong PocotaId { get; }
    EntityState State { get; }
    AccessKind Access { get; }
    EntityProperty? GetEntityProperty(string propertyName);
}
