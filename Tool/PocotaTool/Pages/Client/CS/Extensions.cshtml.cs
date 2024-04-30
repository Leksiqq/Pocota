using Net.Leksi.Pocota.Tool.Client;

namespace Net.Leksi.Pocota.Tool.Pages.Client.CS;

public class ExtensionsModel : ClassModel
{
    public string ContractName { get; set; } = null!;
    public void OnGet()
    {
        CSharpSourceGenerator.PopulateExtensionsModel(this);
    }
}
