using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages;

public class JsonConverterFactoryModel : ClassModel
{
    public List<string> Entities { get; private init; } = [];
    public void OnGet([FromServices] SourceGenerator generator)
    {
        SourceGenerator.PopulateJsonConverterFactoryModel(this);
    }
}
