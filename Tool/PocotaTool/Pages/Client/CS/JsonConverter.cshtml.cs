using Net.Leksi.Pocota.Tool.Client;

namespace Net.Leksi.Pocota.Tool.Pages.CS;

public class JsonConverterModel : ClassModel
{
    public string EntityTypeName { get; set; } = null!;
    public string ContractName { get; set; } = null!;
    public void OnGet()
    {
        CSharpSourceGenerator.PopulateJsonConverterModel(this);
    }
}
