namespace Net.Leksi.Pocota.Tool.Pages;

public class ControllerModel : ClassModel
{
    public string? ServiceClassName { get; set; }
    public string? JsonConverterFactoryClassName { get; set; }
    public void OnGet()
    {
        SourceGenerator.PopulateControllerModel(this);
    }
}
