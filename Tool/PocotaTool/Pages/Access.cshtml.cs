using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages;

public class AccessModel : ClassModel
{
    public string EntityTypeName { get; set; } = null!;
    public string ContractName { get; set; } = null!;
    public void OnGet([FromServices] SourceGenerator generator)
    {
        SourceGenerator.PopulateAccessModel(this);
    }
}
