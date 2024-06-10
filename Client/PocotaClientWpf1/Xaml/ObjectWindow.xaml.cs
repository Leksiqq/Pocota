using System.Windows;
namespace Net.Leksi.Pocota.Client;
public partial class ObjectWindow : Window, IWindowWithCore, IServiceRelated
{
    public WindowCore Core { get; private init; }
    public string ServiceKey { get; private init; }
    public ObjectWindow(string serviceKey, Window owner)
    {
        Core = new WindowCore(this);
        ServiceKey = serviceKey;
        Owner = owner;
        InitializeComponent();
    }
}
