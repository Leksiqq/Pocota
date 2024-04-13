using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages;

public class AccessModel : ClassModel
{
    public void OnGet([FromServices] SourceGenerator generator)
    {
        SourceGenerator.PopulateAccessModel(this);
    }
}
