using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Net.Leksi.Pocota.Client;

public class ApplicationCore(IServiceProvider services): ICommand, INotifyPropertyChanged
{
    private const string s_allWindows = "AllWindows";
    private readonly PropertyChangedEventArgs _propertyChangedEventsArg = new(null);
    private readonly Localizer _localizer = services.GetRequiredService<Localizer>();
    private readonly HashSet<Window> _uniqWindows = [];
    internal Window? ActiveWindow { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

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
    public IEnumerable<object> WindowMenuItems
    {
        get
        {
            yield return new Separator();
            int i = 0;
            foreach (Window window in Application.Current.Windows)
            {
                yield return new Tuple<int, Window>(++i, window);
                if(i == 10)
                {
                    break;
                }
            }
            yield return new MenuItem()
            {
                Header = $"{_localizer.Windows}...",
                Command = this,
                CommandParameter = s_allWindows
            };
        }
    }
    public bool CanExecute(object? parameter)
    {
        return true;
    }
    public void Execute(object? parameter)
    {
        if (parameter is Tuple<int, Window> tup)
        {
            tup.Item2.Activate();
        }
        else if(s_allWindows.Equals(parameter))
        {
            WindowsWindow windowsWindow = new()
            {
                Owner = ActiveWindow
            };
            foreach (Window window in Application.Current.Windows)
            {
                Walk(window, 0, windowsWindow);
            }
            _uniqWindows.Clear();
            windowsWindow.ActiveWindow = ActiveWindow;
            if(windowsWindow.ShowDialog() is bool b && b)
            {
                windowsWindow.ActiveWindow?.Activate();
            }
        }
    }
    internal void Touch()
    {
        PropertyChanged?.Invoke(this, _propertyChangedEventsArg);
    }
    private void Walk(Window window, int v, WindowsWindow windowsWindow)
    {
        if (window != windowsWindow && _uniqWindows.Add(window))
        {
            windowsWindow!.Add(window, v);
            if(window is IWindowWithCore wc)
            {
                foreach (WindowCore core in wc.Core.Launched)
                {
                    Walk(core.Owner, v + 1, windowsWindow);
                }
            }
        }
    }
}
