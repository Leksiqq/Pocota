namespace Net.Leksi.Pocota.Tool.Pages;

public class JsonConverterFactoryModel : ClassModel
{
    public List<string> Entities { get; private init; } = [];
    public void OnGet()
    {
        SourceGenerator.PopulateJsonConverterFactoryModel(this);
    }
}
