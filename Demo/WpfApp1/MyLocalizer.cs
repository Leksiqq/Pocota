using Net.Leksi.Localization;
using Net.Leksi.Pocota.Client;
using System.IO;

namespace WpfApp1;
[ResourcePlace("WpfApp1.Properties.I18nConverter")]
public class MyLocalizer: Localizer
{
    public object? App => GetObject();
}
