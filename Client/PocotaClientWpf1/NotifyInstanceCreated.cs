namespace Net.Leksi.Pocota.Client;
public class NotifyInstanceCreated
{
    public static EventHandler? InstanceCreated;
    private static EventArgs s_instanceCreatedArgs = new();
    public static void Notify(object instance)
    {
        InstanceCreated?.Invoke(instance, s_instanceCreatedArgs);
    }
}
