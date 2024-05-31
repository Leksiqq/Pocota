using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client;
public partial class MethodsWindow : Window, ICommand
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
    private readonly IServiceProvider _services;
    public DataGridManager ConnectorsDataGridManager { get; private init; } = new();
    public WindowsList Windows { get; private init; }
    public MethodsWindow()
    {
        Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
        ConnectorsDataGridManager.ViewSource.Source = _services.GetRequiredService<ConnectorsMethodsList>();
        Windows = _services.GetRequiredService<WindowsList>();
        InitializeComponent();
        Windows.Touch();
    }
    public bool CanExecute(object? parameter)
    {
        return parameter is ConnectorMethod;
    }

    public void Execute(object? parameter)
    {
        if (parameter is ConnectorMethod cm)
        {
            MethodWindow methodWindow = new(cm);
            methodWindow.Show();
        }
    }
    protected override void OnClosed(EventArgs e)
    {
        Windows.Touch();
        base.OnClosed(e);
    }
}
