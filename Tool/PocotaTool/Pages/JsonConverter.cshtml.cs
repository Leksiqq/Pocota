namespace Net.Leksi.Pocota.Tool.Pages;

public class JsonConverterModel : ClassModel
{
    public string EntityTypeName { get; set; } = null!;
    public string ContractName { get; set; } = null!;
    public void OnGet()
    {
        SourceGenerator.PopulateJsonConverterModel(this);
    }
}
