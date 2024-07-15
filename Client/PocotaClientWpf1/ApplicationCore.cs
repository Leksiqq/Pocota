using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;

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
    private readonly HashSet<Window> _uniqWindows = [];
    private readonly Localizer _localizer = Services.GetRequiredService<Localizer>();
    private readonly PropertyChangedEventArgs _windowMenuItemsPropertyChangedEventsArg = new(nameof(WindowMenuItems));
    private Window? _activeWindow = null;
    private static IServiceProvider Services => (Application.Current.Resources[ServiceProviderResourceKey] as IServiceProvider)!;
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
                if (i == 10)
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
        WeakEventManager<Window, EventArgs>.AddHandler(window, "Activated", WindowActivated);
        PropertyChanged?.Invoke(this, _windowMenuItemsPropertyChangedEventsArg);
    }
    private void WindowActivated(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            _activeWindow = window;
        }
    }
}
