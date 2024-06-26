﻿using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Client;

public interface IPocotaEntity
{
    ulong PocotaId { get; }
    EntityState State { get; }
    AccessKind Access { get; }
    IEnumerable<EntityProperty> Properties { get; }
}
