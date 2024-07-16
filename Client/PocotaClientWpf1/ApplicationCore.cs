using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace Net.Leksi.Pocota.Client;
public class ApplicationCore: IValueConverter, ICommand, INotifyPropertyChanged
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
    public event PropertyChangedEventHandler? PropertyChanged;
    private const string s_allWindows = "AllWindows";
    private const int s_maxWindowsInMenu = 10;
    private readonly Localizer _localizer = Application.Current.GetLocalizer();
    private readonly PropertyChangedEventArgs _windowMenuItemsPropertyChangedEventsArg = new(nameof(WindowMenuItems));
    private readonly Dictionary<Window, Window> _launcherByWindow = [];
    private readonly Dictionary<Window, HashSet<Window>> _windowsByLauncher = [];
    private Window? _activeWindow = null;
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
    }
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if ("ThisWindow".Equals(parameter))
        {
            if (value is Tuple<int, Window> tup)
            {
                return tup.Item2 == _activeWindow;
            }
            return false;
        }
        if ("PriorInfo".Equals(parameter))
        {
            if (value is Tuple<int, Window> tup)
            {
                return string.Format("{0,2} ", tup.Item1);
            }
            return string.Empty;
        }
        if ("Title".Equals(parameter))
        {
            if (value is Tuple<int, Window> tup)
            {
                return tup.Item2.Title;
            }
            return value?.ToString() ?? string.Empty;
        }
        if ("AdditionalInfo".Equals(parameter))
        {
            if (value is Tuple<int, Window> tup)
            {
                //return (tup.Item2 as IWindowWithCore)?.Core == _launcher ? $" - {_localizer.Owner}" : ((tup.Item2 as IWindowWithCore)?.Core._launcher == this ? $" - {_localizer.Owned}" : string.Empty);

            }
        }
        return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
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
                if (i == s_maxWindowsInMenu)
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
    public void AttachWindow(Window window, Window? launcher = null)
    {
        if(launcher != null)
        {
            _launcherByWindow.Add(window, launcher);
            if (_windowsByLauncher.TryGetValue(launcher, out var windows))
            {
                windows.Add(window);
            }
            else
            {
                _windowsByLauncher.Add(launcher, [window]);
            }
        }
        //WeakEventManager<Window, EventArgs>.AddHandler(window, "Activated", WindowActivated);
        //WeakEventManager<Window, EventArgs>.AddHandler(window, "Closed", WindowClosed);
        window.Activated += WindowActivated;
        window.Closed += WindowClosed;
        NotifyMenuItemsPropertyChanged();
    }

    private void WindowClosed(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            window.Activated -= WindowActivated;
            window.Closed -= WindowClosed;
            if (_activeWindow == window)
            {
                _activeWindow = null;
            }
            if(_windowsByLauncher.TryGetValue(window, out HashSet<Window>? toClose))
            {
                foreach(var item in toClose)
                {
                    _launcherByWindow.Remove(item);
                }
            }
            if(_launcherByWindow.TryGetValue(window, out var launcher))
            {
                _windowsByLauncher[launcher].Remove(window);
            }
            NotifyMenuItemsPropertyChanged();
            if(toClose != null)
            {
                foreach (var item in toClose)
                {
                    item.Close();
                }
            }
        }
    }

    private void NotifyMenuItemsPropertyChanged()
    {
        PropertyChanged?.Invoke(this, _windowMenuItemsPropertyChangedEventsArg);
    }

    private void WindowActivated(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            _activeWindow = window;
            NotifyMenuItemsPropertyChanged();
        }
    }
}
