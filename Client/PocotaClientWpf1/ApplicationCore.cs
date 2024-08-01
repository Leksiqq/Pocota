using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace Net.Leksi.Pocota.Client;
public class ApplicationCore: DependencyObject, IValueConverter, ICommand
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
    private const string s_allWindows = "AllWindows";
    private const int s_maxWindowsInMenu = 10;
    private static readonly Separator s_separator = new();
    private readonly MenuItem _windowsItem;
    private readonly Localizer _localizer = Application.Current.GetLocalizer();
    private readonly Dictionary<Window, Window> _launcherByWindow = [];
    private readonly Dictionary<Window, HashSet<Window>> _windowsByLauncher = [];
    private Window? _activeWindow = null;
    private int _entersCount = 0;
    public bool NeedToCheckAndShowLeaks { get; set; } = false;
    public ObservableCollection<object> WindowMenuItems { get; private init; } = [];
    public ApplicationCore()
    {
        _windowsItem = new()
        {
            Header = $"{_localizer.Windows}...",
            Command = this,
            CommandParameter = s_allWindows
        };
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
        WeakEventManager<Window, EventArgs>.AddHandler(window, "Activated", WindowActivated);
        WeakEventManager<Window, EventArgs>.AddHandler(window, "Closed", WindowClosed);
    }
    public Window? GetLauncher(Window window)
    {
        return _launcherByWindow.TryGetValue(window, out Window? launcher) ? launcher : null;
    }
    private void WindowClosed(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            ++_entersCount;
            WeakEventManager<Window, EventArgs>.RemoveHandler(window, "Activated", WindowActivated);
            WeakEventManager<Window, EventArgs>.RemoveHandler(window, "Closed", WindowClosed);
            if (_activeWindow == window)
            {
                _activeWindow = null;
            }
            if(_launcherByWindow.TryGetValue(window, out Window? launcher) && _windowsByLauncher.TryGetValue(launcher, out HashSet<Window>? launched))
            {
                launched.Remove(window);
            }
            _launcherByWindow.Remove(window);
            if (_windowsByLauncher.TryGetValue(window, out HashSet<Window>? toClose))
            {
                _windowsByLauncher.Remove(window);
                foreach (var item in toClose)
                {
                    item.Close();
                }
            }
            if (--_entersCount == 0)
            {
                RefreshWindowMenuItems();
                if (NeedToCheckAndShowLeaks)
                {
                    int numLeaks = 0;
                    foreach (
                        var w in
                        _windowsByLauncher.Keys.Concat(_launcherByWindow.Values).Concat(_launcherByWindow.Keys)
                            .Concat(_windowsByLauncher.Values.SelectMany(v => v)).ToHashSet()
                    )
                    {
                        Console.Write($"{w.Title} ");
                        bool found = false;
                        foreach (var w1 in Application.Current.Windows)
                        {
                            if (w1 == w)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            ++numLeaks;
                        }
                    }
                    Console.WriteLine($"\nnumLeaks: {numLeaks}, numWin: {Application.Current.Windows.Count}");
                    if(numLeaks > 0)
                    {
                        Environment.Exit(numLeaks);
                    }
                }
            }
        }
    }
    private void WindowActivated(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            _activeWindow = window;
            RefreshWindowMenuItems();
        }
    }
    private void RefreshWindowMenuItems()
    {
        WindowMenuItems.Clear();
        WindowMenuItems.Add(s_separator);
        int i = 0;
        foreach (Window window in Application.Current.Windows)
        {
            WindowMenuItems.Add(new Tuple<int, Window>(++i, window));
            if (i == s_maxWindowsInMenu)
            {
                break;
            }
        }
        WindowMenuItems.Add(_windowsItem);
    }
}
