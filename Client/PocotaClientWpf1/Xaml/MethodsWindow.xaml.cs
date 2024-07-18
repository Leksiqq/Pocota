using Net.Leksi.Util;
using Net.Leksi.WpfMarkup;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
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
    public DataGridManager ConnectorsDataGridManager { get; private init; } = new();
    public MethodsWindow()
    {
        ConnectorsDataGridManager.ViewSource.Source = Application.Current.GetRequiredService<ConnectorsMethodsList>();
        Activated += MethodsWindow_Activated;
        InitializeComponent();
    }
    private Random rnd = new();
    private void MethodsWindow_Activated(object? sender, EventArgs e)
    {
        WindowsShower.Dispatcher = Dispatcher;
        WindowsShower.Start(() =>
        {
            MethodWindow window = Application.Current.GetRequiredService<MethodWindow>();
            window.Init(
                Application.Current.GetRequiredService<ConnectorsMethodsList>()
                .Skip(rnd.Next(Application.Current.GetRequiredService<ConnectorsMethodsList>().Count() - 1))
                .First()
            );
            return window;
        });
        LifetimeVisualizer.Start();
    }
    public bool CanExecute(object? parameter)
    {
        return parameter is ConnectorMethod;
    }
    public void Execute(object? parameter)
    {
        if (parameter is ConnectorMethod cm)
        {
            MethodWindow methodWindow = Application.Current.GetRequiredService<MethodWindow>();
            Application.Current.AttachWindow(methodWindow, this);
            methodWindow.Init(cm);
            methodWindow.Show();
        }
    }
}
