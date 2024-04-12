namespace Net.Leksi.Pocota.Server;

public class PocotaEntity
{
    private readonly HashSet<int> _sentProperties = [];
    public ulong PocotaId { get; internal set; }
    public bool IsSerialized { get; private set; } = false;
    public void MarkPropertySent(int i)
    {
        _sentProperties.Add(i);
    }
    public bool IsPropertySent(int i)
    {
        return _sentProperties.Contains(i);
    }
    public void SetSerialized()
    {
        IsSerialized = true;
    }
}
