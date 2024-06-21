using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
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
    public DataGridManager ConnectorsDataGridManager { get; private init; } = new();
    public MethodsWindow()
    {
        ConnectorsDataGridManager.ViewSource.Source = ((IServiceProvider)Application.Current.Resources[ServiceProviderResourceKey]).GetRequiredService<ConnectorsMethodsList>();
        Activated += MethodsWindow_Activated;
        InitializeComponent();
    }

    Timer? t;
    internal ulong count = 0;
    internal ulong step = 0;
    private void MethodsWindow_Activated(object? sender, EventArgs e)
    {
        SemaphoreSlim ss = new(1);
        if (t is null)
        {
            t = new Timer(s =>
            {
                if (ss.Wait(1))
                {
                    Dispatcher.Invoke(() =>
                    {
                        //ObjectWindow ow = new ObjectWindow("Pizza", this);
                        Window1 ow = new(this);
                        Interlocked.Increment(ref count);
                        if (Interlocked.Increment(ref step) % 100 == 0)
                        {
                            GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true, true);
                            GC.WaitForPendingFinalizers();
                        }
                        Console.Write($"\r              \r{count}");
                        ow.Show();
                        ow.Close();
                    });
                    ss.Release();
                }
            }, null, 0, 10);
        }
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
