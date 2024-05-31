using Net.Leksi.Localization;
using System.Diagnostics;

namespace Net.Leksi.Pocota.Client;

[ResourcePlace("Net.Leksi.Pocota.Client.Properties.I18nConverter")]
public class Localizer: LocalizationBase
{
    public string MethodsWindowTitle => GetString();
    public string Method => GetString();
    public string Connector => GetString();
    public string Module => GetString();
    public string Ascending => GetString();
    public string Descending => GetString();
    public string Unsorted => GetString();
    public string Unsort => GetString();
    public string SortPosition => GetString();
    public string Call => GetString();
    public string Clear => GetString();
    public string Edit => GetString();
    public string Create => GetString();
    public string InsertBefore => GetString();
    public string Move => GetString();
    public string Find => GetString();
    public string Windows => GetString();
    public string ShowNamespace => GetString();
    public string HideNamespace => GetString();
    public string IsNotSet => GetString();
    public string Count => GetString();
    public string IsSet => GetString();
    public string GoToLauncher => GetString();
    public string Add => GetString();
    public string PlaceLast => GetString();
    public string Remove => GetString();
    public string CancelMove => GetString();
    public string PlaceBefore => GetString();
    public string Property => GetString();
    public string Value => GetString();
    public string Actions => GetString();
    public string Position => GetString();
    public string Parameter => GetString();
    public string Check => GetString();
    public string ServiceKey => GetString();
    public Localizer()
    {
    }
}
