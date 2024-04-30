namespace Net.Leksi.Pocota.Client;

public class EntityProperty
{
    public Type EntityType { get; private init; }
    public string Name { get; private init; }
    public PropertyState State { get; internal set; }
    public EntityProperty(Type entityType, string name)
    {
        EntityType = entityType;
        Name = name;
    }
}
