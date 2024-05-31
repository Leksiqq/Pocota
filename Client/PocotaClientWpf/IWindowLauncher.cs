using System.ComponentModel;

namespace Net.Leksi.Pocota.Client;

public interface IWindowLauncher: INotifyPropertyChanged
{
    EditWindowLauncher Launcher { get; }
    bool KeysOnly { get; set; }
    string ServiceKey { get; set; }
}
