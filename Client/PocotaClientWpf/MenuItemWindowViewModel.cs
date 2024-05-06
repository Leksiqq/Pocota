using System.Windows.Input;

namespace Net.Leksi.Pocota.Client;

public class MenuItemWindowViewModel
{
    public string Header { get; set; } = null!;
    public ICommand? Command { get; set; }
}
