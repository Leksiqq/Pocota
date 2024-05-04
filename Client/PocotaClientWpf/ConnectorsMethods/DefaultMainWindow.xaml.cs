using Net.Leksi.WpfMarkup;
using System.Windows;

namespace Net.Leksi.Pocota.Client;
public partial class DefaultMainWindow : Window
{
    public DataGridManager ConnectorsDataGridManager { get; private init; } = new();
    public RunMethodCommand RunCommand { get; private init; }
    public DefaultMainWindow()
    {
        RunCommand = new RunMethodCommand();
        InitializeComponent();
        Console.WriteLine(((BindingProxy)Application.Current.Resources["ServiceProvider"]).Value);
        ConnectorsDataGridManager.ViewSource.Source = new ConnectorsMethodsList();
    }

 }
