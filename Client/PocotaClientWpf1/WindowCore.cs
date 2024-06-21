using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;
public class WindowCore : IValueConverter
{
    private readonly Window _owner;
    private readonly Localizer _localizer;
    private readonly List<WeakReference<WindowCore>> _launched = [];
    private WindowCore? _launcher = null;
    public ApplicationCore ApplicationCore => null;// Services.GetRequiredService<ApplicationCore>();
    public IServiceProvider Services => null;// (IServiceProvider)Application.Current.Resources[ServiceProviderResourceKey];
    public WindowCore? Launcher
    {
        get => _launcher;
        set
        {
            if(_launcher is null && value is { })
            {
                _launcher = value;
                _owner.Owner = _launcher._owner;
                _launcher._launched.Add(new WeakReference<WindowCore>(this));
            }
        }
    }
    public IEnumerable<WindowCore> Launched => _launched.Select(wr => wr.TryGetTarget(out WindowCore? wc) ?  wc : null)
        .Where(wc => wc is { } && wc._owner.IsLoaded)!;
    public Window Owner => _owner;
    internal WindowCore(Window owner)
    {
        _owner = owner;
        _owner.SizeToContent = SizeToContent.WidthAndHeight;
        _owner.Closed += Owner_Closed;
        _owner.Activated += Owner_Activated;
        _owner.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        _localizer = ((IServiceProvider)Application.Current.Resources[ServiceProviderResourceKey]).GetRequiredService<Localizer>();
        ((IServiceProvider)Application.Current.Resources[ServiceProviderResourceKey]).GetRequiredService<ApplicationCore>().Touch();
    }
    private void Owner_Activated(object? sender, EventArgs e)
    {
        ((IServiceProvider)Application.Current.Resources[ServiceProviderResourceKey]).GetRequiredService<ApplicationCore>().ActiveWindow = _owner;
        _owner.Owner = null;
    }
    private void Owner_Closed(object? sender, EventArgs e)
    {
        foreach(WeakReference<WindowCore> wr in _launched)
        {
            if(wr.TryGetTarget(out WindowCore? core))
            {
                core.Owner.Close();
            }
        }
        ((IServiceProvider)Application.Current.Resources[ServiceProviderResourceKey]).GetRequiredService<ApplicationCore>().Touch();
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("ThisWindow".Equals(parameter))
        {
            if (value is Tuple<int, Window> tup)
            {
                return tup.Item2 == _owner;
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
                return (tup.Item2 as IWindowWithCore)?.Core == _launcher ? $" - {_localizer.Owner}" : ((tup.Item2 as IWindowWithCore)?.Core._launcher == this ? $" - {_localizer.Owned}" : string.Empty);

            }
        }
        return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
