using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages;

public class ServiceBaseModel : ClassModel
{
    public void OnGet([FromServices] SourceGenerator generator)
    {
        generator.PopulateServiceBaseModel(this);
    }
}
