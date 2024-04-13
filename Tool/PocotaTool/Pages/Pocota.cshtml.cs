using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages;

public class PocotaModel : ClassModel
{
    public void OnGet([FromServices] SourceGenerator generator)
    {
        SourceGenerator.PopulatePocotaModel(this);
    }
}
