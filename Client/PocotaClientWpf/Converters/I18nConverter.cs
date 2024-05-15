using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class I18nConverter : Freezable, IValueConverter
{
    private readonly List<ResourceManager> _resourceManagers = [];
    private readonly Dictionary<string, string?> _cache = [];

    public void AddResourceManager(ResourceManager resourceManager)
    {
        _resourceManagers.Add(resourceManager);
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string ask = value.ToString()!;
        if(!_cache.TryGetValue(ask, out string? ans))
        {
            foreach (ResourceManager rm in _resourceManagers)
            {
                if (rm.GetString(ask) is string s)
                {
                    ans = s;
                }
            }
            _cache[ask] = ans;
        }
        return ans ?? value;
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
