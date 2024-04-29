using Net.Leksi.Pocota.Tool.Client;

namespace Net.Leksi.Pocota.Tool.Pages.Client.CS;

public class ConnectorModel: ClassModel
{
    public string? ServiceClassName { get; set; }
    public string? JsonConverterFactoryClassName { get; set; }
    public string ContractName { get; set; } = null!;
    public void OnGet()
    {
        CSharpSourceGenerator.PopulateConnectorModel(this);
    }
}
