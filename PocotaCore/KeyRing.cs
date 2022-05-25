namespace Net.Leksi.Pocota.Core;

public class KeyRing
{
    private readonly PocotaManager _manager;
    internal object? Source { get; set; } = null;

    public object[] Key { get; internal set; } = null!;
    public bool IsNew { get; internal set; } = true;

    public KeyRing(PocotaManager manager)
    {
        _manager = manager;
    }

    public object this[string fieldName]
    {
        get
        {
            int index = _manager.GetFieldIndex(Source!.GetType(), fieldName);
            if (index >= 0)
            {
                return Key[index];
            }
            throw new IndexOutOfRangeException();
        }
        set
        {
            int index = _manager.GetFieldIndex(Source!.GetType(), fieldName);
            if(index >= 0)
            {
                Key[index] = value;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
