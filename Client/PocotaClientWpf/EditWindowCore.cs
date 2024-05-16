using System.Windows;

namespace Net.Leksi.Pocota.Client;

public class EditWindowCore
{
    public string Path { get; protected set; } = null!;
    public string ObjectType { get; protected set; } = null!;
    internal EditWindowCore(string path, Type type)
    {
        Path = path;
        ObjectType = Util.BuildTypeFullName(type);
    }
}
