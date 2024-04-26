namespace Net.Leksi.Pocota.Tool.Pages;

public class ExtensionsModel : ClassModel
{
    public string ContractName { get; set; } = null!;
    public void OnGet()
    {
        SourceGenerator.PopulateExtensionsModel(this);
    }
}
