using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages;

public class PocotaModel : ClassModel
{
    public string ContractName { get; set; } = null!;
    public void OnGet([FromServices] SourceGenerator generator)
    {
        SourceGenerator.PopulatePocotaModel(this);
    }
}
