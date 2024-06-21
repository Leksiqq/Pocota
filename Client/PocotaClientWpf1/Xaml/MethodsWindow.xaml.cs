using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System.Windows;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;
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
        ConnectorsDataGridManager.ViewSource.Source = ((IServiceProvider)Application.Current.Resources[ServiceProviderResourceKey]).GetRequiredService<ConnectorsMethodsList>();
        InitializeComponent();
        Activated += MethodsWindow_Activated;
    }

    Timer t;
    internal ulong count = 0;
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
                        ObjectWindow ow = new ObjectWindow("Pizza", this);
                        //Window1 ow = new Window1(this);
                        if(Interlocked.Increment(ref count) > 100)
                        {
                            GC.Collect();
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
