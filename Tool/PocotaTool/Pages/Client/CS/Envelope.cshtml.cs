using Net.Leksi.Pocota.Tool.Client;

namespace Net.Leksi.Pocota.Tool.Pages.Client.CS;

public class EnvelopeModel : ClassModel
{
    public void OnGet()
    {
        CSharpSourceGenerator.PopulateClientEnvelope(this);
    }
}
