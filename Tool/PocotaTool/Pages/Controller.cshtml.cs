using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages;

public class ControllerModel : ClassModel
{
    public string? ServiceClassName { get; set; }
    public string? JsonConverterFactoryClassName { get; set; }
    public void OnGet([FromServices] SourceGenerator generator)
    {
        SourceGenerator.PopulateControllerModel(this);
    }
}
