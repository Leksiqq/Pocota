using Net.Leksi.Pocota.Tool.Client;

namespace Net.Leksi.Pocota.Tool.Pages.Client.CS;

public class PocotaContextModel : ClassModel
{
    public List<string> Entities { get; private init; } = [];
    public void OnGet()
    {
        CSharpSourceGenerator.PopulatePocotaContextModel(this);
    }
}
