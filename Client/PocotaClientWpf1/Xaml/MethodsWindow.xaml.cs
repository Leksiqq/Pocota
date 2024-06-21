using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
namespace Net.Leksi.Pocota.Client;
public partial class MethodsWindow : Window, IWindowWithCore, ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }
    public WindowCore Core { get; private init; }
    public DataGridManager ConnectorsDataGridManager { get; private init; } = new();
    public MethodsWindow()
    {
        Core = new WindowCore(this);
        ConnectorsDataGridManager.ViewSource.Source = Core.Services.GetRequiredService<ConnectorsMethodsList>();
        InitializeComponent();
    }
    public bool CanExecute(object? parameter)
    {
        return parameter is ConnectorMethod;
    }
    public void Execute(object? parameter)
    {
        if (parameter is ConnectorMethod cm)
        {
            MethodWindow methodWindow = new(cm, this);
            methodWindow.Show();
        }
    }
}
