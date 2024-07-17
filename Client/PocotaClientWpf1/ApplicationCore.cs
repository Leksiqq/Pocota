using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace Net.Leksi.Pocota.Client;
public class ApplicationCore: DependencyObject, IValueConverter, ICommand, INotifyPropertyChanged
{
    public static readonly DependencyProperty WindowMenuItemsProperty = DependencyProperty.Register(
       nameof(WindowMenuItems), typeof(ObservableCollection<object>), typeof(ApplicationCore)
    );
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
    private static readonly Separator s_separator = new();
    private readonly MenuItem _windowsItem;
    private readonly Localizer _localizer = Application.Current.GetLocalizer();
    private readonly PropertyChangedEventArgs _windowMenuItemsPropertyChangedEventsArg = new(nameof(WindowMenuItems));
    private readonly Dictionary<Window, Window> _launcherByWindow = [];
    private readonly Dictionary<Window, HashSet<Window>> _windowsByLauncher = [];
    private readonly ObservableCollection<object> _windowMenuItems = [];
    private Window? _activeWindow = null;
    private int _entersCount = 0;
    public ApplicationCore()
    {
        _windowsItem = new()
        {
            Header = $"{_localizer.Windows}...",
            Command = this,
            CommandParameter = s_allWindows
        };
        SetValue(WindowMenuItemsProperty, _windowMenuItems);
    }
    public IEnumerable<object> WindowMenuItems
    {
        get => (IEnumerable<object>)GetValue(WindowMenuItemsProperty);
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
            if (_activeWindow != null && value is Tuple<int, Window> tup)
            {
                if (_launcherByWindow.TryGetValue(_activeWindow, out Window? win) && win == tup.Item2)
                {
                    return $" - {_localizer.Owner}";
                }
                if (_windowsByLauncher.TryGetValue(_activeWindow, out var wins) && wins.Contains(tup.Item2))
                {
                    return $" - {_localizer.Owned}";
                }
                return string.Empty;
            }
        }
        return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
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
        window.Resources[Constants.Localizer] = Application.Current.GetLocalizer();
        window.Activated += WindowActivated;
        window.Closed += WindowClosed;
        //NotifyWindowMenuItemsPropertyChanged();
    }
    private void WindowClosed(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            ++_entersCount;
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
            if(toClose != null)
            {
                foreach (var item in toClose)
                {
                    item.Close();
                }
            }
            if(--_entersCount == 0)
            {
                NotifyWindowMenuItemsPropertyChanged();
            }
        }
    }
    private void WindowActivated(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            _activeWindow = window;
            NotifyWindowMenuItemsPropertyChanged();
        }
    }
    private void NotifyWindowMenuItemsPropertyChanged()
    {
        _windowMenuItems.Clear();
        _windowMenuItems.Add(s_separator);
        int i = 0;
        foreach (Window window in Application.Current.Windows)
        {
            _windowMenuItems.Add(new Tuple<int, Window>(++i, window));
            if (i == s_maxWindowsInMenu)
            {
                break;
            }
        }
        _windowMenuItems.Add(_windowsItem);

        //PropertyChanged?.Invoke(this, _windowMenuItemsPropertyChangedEventsArg);
    }
}
