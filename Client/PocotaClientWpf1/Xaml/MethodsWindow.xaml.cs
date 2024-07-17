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

    private Timer? t = null;
    private volatile int count = 0;
    private volatile int step = 0;
    private volatile bool needNewLine = false;
    private readonly Random rnd = new();
    private void MethodsWindow_Activated(object? sender, EventArgs e)
    {
        SemaphoreSlim ss = new(1);
        if (t is null)
        {
            LifetimeObserver lifetimeObserver = Application.Current.GetRequiredService<LifetimeObserver>();
            lifetimeObserver.LifetimeEventOccured += (s, e) =>
            {
                switch (e.Kind)
                {
                    case LifetimeEventKind.Created:
                        Interlocked.Increment(ref count);
                        break;
                    case LifetimeEventKind.Finalized:
                        Interlocked.Decrement(ref count);
                        needNewLine = true;
                        break;
                };
            };
            t = new Timer(s =>
            {
                if (ss.Wait(1))
                {
                    Dispatcher.Invoke(() =>
                    {
                        MethodWindow methodWindow = Application.Current.GetRequiredService<MethodWindow>();
                        methodWindow.Init(
                            Application.Current.GetRequiredService<ConnectorsMethodsList>()
                                .Skip(rnd.Next(Application.Current.GetRequiredService<ConnectorsMethodsList>().Count() - 1))
                                .First()
                        );

                        //Window1 methodWindow = Application.Current.GetRequiredService<Window1>();
                        
                        Application.Current.AttachWindow(methodWindow);
                        if (Interlocked.Increment(ref step) % 100 == 0)
                        {
                            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, false);
                            GC.WaitForPendingFinalizers();
                            Thread.Sleep(1000);
                        }
                        if (needNewLine)
                        {
                            Console.WriteLine();
                            needNewLine = false;
                        }
                        Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                        Console.Write($"\r              \r{count}    {currentProcess.WorkingSet64}");
                        methodWindow.Show();
                        methodWindow.Close();
                        methodWindow.DataContext = null;
                        ss.Release();
                    });
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
            MethodWindow methodWindow = Application.Current.GetRequiredService<MethodWindow>();
            Application.Current.AttachWindow(methodWindow, this);
            methodWindow.Init(cm);
            methodWindow.Show();
        }
    }
}
