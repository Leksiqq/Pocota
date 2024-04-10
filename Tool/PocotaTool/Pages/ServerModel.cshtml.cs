using Microsoft.AspNetCore.Mvc;

namespace Net.Leksi.Pocota.Tool.Pages
{
    internal class ServerModelModel : ClassModel
    {
        public void OnGet([FromServices]SourceGenerator generator)
        {
            generator.GenerateServerModel(this);
        }
    }
}
