﻿namespace Net.Leksi.Pocota.Client;

public interface IPocotaEntity
{
    ulong PocotaId { get; }
    EntityState State { get; }
}
