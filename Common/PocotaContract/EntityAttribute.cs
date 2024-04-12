namespace Net.Leksi.Pocota.Contract;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
public class EntityAttribute: Attribute
{
    private readonly Type _entityType;
    public Type EntityType => _entityType;
    public string? NameOfSet {  get; set; }
    public EntityAttribute(Type entityType)
    {
        _entityType = entityType;
        if (!_entityType.IsClass)
        {
            throw new ArgumentException($"{_entityType} is not a class.");
        }
    }
}
