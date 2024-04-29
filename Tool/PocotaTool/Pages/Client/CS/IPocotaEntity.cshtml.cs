using Net.Leksi.Pocota.Tool.Client;

namespace Net.Leksi.Pocota.Tool.Pages.CS;

public class IPocotaEntityModel : ClassModel
{
    public string ContractName { get; set; } = null!;
    public void OnGet()
    {
        CSharpSourceGenerator.PopulatePocotaEntityModel(this);
    }
}
