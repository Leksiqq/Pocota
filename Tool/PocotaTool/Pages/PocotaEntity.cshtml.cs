using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages;

public class PocotaEntityModel : ClassModel
{
    public string ContractName { get; set; } = null!;
    public void OnGet([FromServices] SourceGenerator generator)
    {
        SourceGenerator.PopulatePocotaEntityModel(this);
    }
}
