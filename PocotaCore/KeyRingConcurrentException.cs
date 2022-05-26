namespace Net.Leksi.Pocota.Core;

public class KeyRingConcurrentException: Exception
{
    public KeyRingConcurrentException() : base() { }
    public KeyRingConcurrentException(string message) : base(message) { }
}
