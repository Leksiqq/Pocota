using Microsoft.EntityFrameworkCore.ChangeTracking;
using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Server;

public class EntityPropertyNavigation: EntityProperty
{
    private readonly NavigationEntry _entry;
    public EntityPropertyNavigation(PocotaEntity pocotaEntity, NavigationEntry navigationEntry): base(pocotaEntity)
    {
        _entry = navigationEntry;
    }
    public override AccessKind Access 
    {
        get
        {
            return _entry.IsLoaded ? base.Access : AccessKind.NotSet;
        }
        set => base.Access = value; 
    }
    public void Load()
    {
        _entry.Load();
    }
}
