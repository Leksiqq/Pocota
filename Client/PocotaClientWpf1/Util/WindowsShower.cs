using System.Windows;
using System.Windows.Threading;
namespace Net.Leksi.Pocota.Client;
public static class WindowsShower
{
    private static volatile bool _running = false;
    private static Timer? _timer = null;
    private static readonly HashSet<Timer> _timers = [];
    public static Dispatcher? Dispatcher { get; set; }
    public static int TimeOfWindowExpositionMs { get; set; } = 3000;
    public static int PeriodOfShowMs { get; set; } = 1000;
    public static void Start(Func<Window> supplier)
    {
        if(supplier == null)
        {
            throw new ArgumentNullException(nameof(supplier));
        }
        if(Dispatcher == null)
        {
            throw new InvalidOperationException("No Dispatcher!");
        }
        if(!_running)
        {
            _running = true;
            _timer = new Timer(s =>
            {
                Dispatcher!.Invoke(() =>
                {
                    Window window = supplier.Invoke();

                    Application.Current.AttachWindow(window);
                    window.Activated += (s, e) =>
                    {
                        Timer t1 = new(state =>
                        {
                            Dispatcher.Invoke(() =>
                            {
                                (s as Window)!.Close();
                            });
                            _timers.Remove((state as Timer)!);
                        });
                        t1.Change(TimeOfWindowExpositionMs, 0);
                        _timers.Add(t1);
                    };
                    window.Show();
                });
                if (_running)
                {
                    (s as Timer)!.Change(PeriodOfShowMs, 0);
                }
            });
            _timer.Change(0, 0);
        }
    }
    public static void Stop()
    {
        _running = false;
    }

}
