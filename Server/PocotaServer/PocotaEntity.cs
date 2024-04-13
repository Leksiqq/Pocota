namespace Net.Leksi.Pocota.Server;

public class PocotaEntity
{
    private bool _isSerialized = false;
    public ulong PocotaId { get; internal set; }
    public bool IsSerialized 
    { 
        get => _isSerialized; 
        set
        {
            if (!value)
            {
                throw new InvalidOperationException();
            }
            if (!_isSerialized && value)
            {
                _isSerialized = true;
            }
        }
    }
}
