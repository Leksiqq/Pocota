using Net.Leksi.E6dWebApp;

namespace Net.Leksi.Pocota.Tool;

internal interface IClientSourceGenerator
{
    Task GenerateConnectorAsync(IConnector connector, Type contractType, string targetFolder, string name);
    Task GenerateModelAsync(IConnector connector, Type contractType, Type entityType, string targetFolder);
    Task GenerateExtensionsAsync(IConnector connector, Type contractType, string targetFolder, string name);
}
