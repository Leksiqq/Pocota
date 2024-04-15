using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages;

public class DbContextModel : ClassModel
{
    public void OnGet([FromServices] SourceGenerator generator)
    {
        SourceGenerator.PopulateContextModel(this);
    }
}
