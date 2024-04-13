using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Server;

public class EntityProperty
{
    private PropertyAccess _propertyAccess = PropertyAccess.Full;
    private bool _propertyAccessSet = false;
    private bool _isSent = false;
    public PropertyAccess Access 
    { 
        get => _propertyAccess; 
        set
        {
            if (!_propertyAccessSet)
            {
                _propertyAccess = value;
                _propertyAccessSet = true;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
    public bool IsSent 
    { 
        get => _isSent; 
        set
        {
            if (!value)
            {
                throw new InvalidOperationException();
            }
            if (!_isSent && value)
            {
                _isSent = true;
            }
        }
    }
}
