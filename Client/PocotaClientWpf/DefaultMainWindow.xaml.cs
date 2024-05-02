using Net.Leksi.WpfMarkup;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;
public partial class DefaultMainWindow : Window
{
    public CollectionViewSource ConnectorViewSource { get; private init; } = new();
    public SortCommand SortCommand { get; private init; }
    public DefaultMainWindow()
    {
        SortCommand = new(ConnectorViewSource);
        InitializeComponent();
        Console.WriteLine(((BindingProxy)Application.Current.Resources["ServiceProvider"]).Value);
        ConnectorViewSource.Source = new ConnectorsMethodsList();
    }

}
