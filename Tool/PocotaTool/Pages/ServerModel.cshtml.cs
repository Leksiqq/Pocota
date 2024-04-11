using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages;

public class ServerModelModel : ClassModel
{
    public void OnGet([FromServices]SourceGenerator generator)
    {
        SourceGenerator.PopulateServerModelModel(this);
    }
}
