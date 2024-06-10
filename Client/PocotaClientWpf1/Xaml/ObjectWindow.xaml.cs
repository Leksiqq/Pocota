using System.ComponentModel;
using System.Windows;
namespace Net.Leksi.Pocota.Client;
public partial class ObjectWindow : Window, IWindowWithCore, IServiceRelated, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private Property? _property;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    public WindowCore Core { get; private init; }
    public string ServiceKey { get; private init; }
    public Property? Property 
    { 
        get => _property; 
        internal set
        {
            if (_property != value)
            {
                _property = value;
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
        } 
    }
    public ObjectWindow(string serviceKey, Window owner)
    {
        Core = new WindowCore(this);
        ServiceKey = serviceKey;
        Owner = owner;
        InitializeComponent();
    }
}
