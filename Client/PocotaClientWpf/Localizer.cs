using Net.Leksi.Localization;

namespace Net.Leksi.Pocota.Client;

[ResourcePlace("Net.Leksi.Pocota.Client.Properties.I18nConverter")]
public class Localizer: Core
{
    public virtual string MethodsWindowTitle => GetString();
}
