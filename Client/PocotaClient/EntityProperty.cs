namespace Net.Leksi.Pocota.Client;

public abstract class EntityProperty(PocotaEntity entity, string name, Type type) : Property(name, type)
{
    public PocotaEntity Entity { get; private init; } = entity;
    public PropertyState State { get; internal set; }
}
