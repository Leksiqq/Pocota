using Net.Leksi.Pocota.Tool.Client;

namespace Net.Leksi.Pocota.Tool.Pages.Client.CS;

public class ModelModel : ClassModel
{
    public void OnGet()
    {
        CSharpSourceGenerator.PopulateClientModel(this);
    }
}
