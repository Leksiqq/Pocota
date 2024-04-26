namespace Net.Leksi.Pocota.Tool.Pages;

public class DbContextModel : ClassModel
{
    public void OnGet()
    {
        SourceGenerator.PopulateContextModel(this);
    }
}
