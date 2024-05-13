namespace Net.Leksi.Pocota.Client;

public class EntityProperty
{
    public PocotaEntity Entity { get; private init; }
    public string Name { get; private init; }
    public PropertyState State { get; internal set; }
    public EntityProperty(PocotaEntity entity, string name)
    {
        Entity = entity;
        Name = name;
    }
}
