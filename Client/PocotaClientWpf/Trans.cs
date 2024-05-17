using System.Globalization;
using System.Windows;

namespace Net.Leksi.Pocota.Client;
public class Trans(Application app)
{
    public string Translate(string source)
    {
        return (string)(app.Resources[Constants.I18nConverter] as I18nConverter)?
            .Convert(source, typeof(string), null!, CultureInfo.InvariantCulture)!;
    }
}
