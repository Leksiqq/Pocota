using Microsoft.EntityFrameworkCore.ChangeTracking;
using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Server;

public class EntityPropertyNavigation: EntityProperty
{
    private readonly NavigationEntry _entry;
    public override object? NotSetStub
    {
        get
        {
            if (!_entry.IsLoaded && !_entry.Metadata.IsCollection && _entry.Metadata.TargetEntityType.FindPrimaryKey() is var key)
            {
                Console.WriteLine(key.DeclaringEntityType.Name);
                foreach (var p in key.Properties)
                {
                    Console.WriteLine($"{p.Name}");
                }
            }
            return null;
        }
    }
    public EntityPropertyNavigation(PocotaEntity pocotaEntity, NavigationEntry navigationEntry): base(pocotaEntity)
    {
        _entry = navigationEntry;
        if(navigationEntry.Metadata.TargetEntityType.FindPrimaryKey() is var key)
        {
            Console.WriteLine(key.DeclaringEntityType);
            foreach(var p in key.Properties)
            {
                Console.WriteLine($"{p.Name}");
            }
        }
    }
    public override PropertyAccess Access 
    {
        get
        {
            return _entry.IsLoaded ? base.Access : PropertyAccess.NotSet;
        }
        set => base.Access = value; 
    }
    public void Load()
    {
        _entry.Load();
    }
}
