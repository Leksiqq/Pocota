using System.ComponentModel;
using System.Windows;

namespace Net.Leksi.Pocota.Client;

public interface IWindowLauncher: INotifyPropertyChanged
{
    EditWindowLauncher Launcher { get; }
}
