using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Net.Leksi.Pocota.Server;

public class EntityPropertyProperty: EntityProperty
{
    private readonly PropertyEntry _entry;
    public EntityPropertyProperty(PocotaEntity pocotaEntity, PropertyEntry propertyEntry): base(pocotaEntity)
    {
        _entry = propertyEntry;
        if (_entry.Metadata.IsPrimaryKey())
        {
            _propertyAccess = Contract.PropertyAccess.Key;
        }
    }
}
