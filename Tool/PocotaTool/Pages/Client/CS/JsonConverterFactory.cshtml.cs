using Net.Leksi.Pocota.Tool.Client;

namespace Net.Leksi.Pocota.Tool.Pages.CS;

public class JsonConverterFactoryModel : ClassModel
{
    public List<string> Entities { get; private init; } = [];
    public void OnGet()
    {
        CSharpSourceGenerator.PopulateJsonConverterFactoryModel(this);
    }
}
