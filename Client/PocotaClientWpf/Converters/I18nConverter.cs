using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Data;

[assembly: RootNamespace("Net.Leksi.Pocota.Client")]

namespace Net.Leksi.Pocota.Client;

public class I18nConverter(IServiceProvider services) : Freezable, IValueConverter
{
    private readonly List<Func<IServiceProvider, IStringLocalizer>> _localizerFinders = [];
    private readonly Dictionary<string, string?> _cache = [];
    public bool ShowLabels { get; set; } = false;
    public void AddLocalizerFinder(Func<IServiceProvider, IStringLocalizer> finder)
    {
        _localizerFinders.Add(finder);
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string ask = value.ToString()!;
        if (!_cache.TryGetValue(ask, out string? ans))
        {
            foreach (Func<IServiceProvider, IStringLocalizer> finder in _localizerFinders)
            {
                IStringLocalizer localizer = finder(services);
                LocalizedString ls = localizer.GetString(ask);
                Console.WriteLine($"{ask}: {ls}");
                if (!ls.ResourceNotFound)
                {
                    ans = ls.Value;
                }
            }
            _cache[ask] = ans;
        }
        if(ShowLabels && ans is string)
        { 
            return $"{ans} [{value}]";
        }
        return ans ?? $"[{value}]";
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}
