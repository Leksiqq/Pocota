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
        if (!_entityType.IsInterface)
        {
            throw new ArgumentException($"{_entityType} is not an interface.");
        }
        if (!_entityType.Name.StartsWith('I'))
        {
            throw new ArgumentException($"{_entityType} name is not starts with 'I'.");
        }
    }
}
