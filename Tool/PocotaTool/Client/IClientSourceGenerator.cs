using Net.Leksi.E6dWebApp;

namespace Net.Leksi.Pocota.Tool;

internal interface IClientSourceGenerator
{
    Task GenerateConnectorAsync(IConnector connector, Type contractType, string targetFolder, string name, HashSet<Type> envelopes, HashSet<Type> entities);
    Task GenerateModelAsync(IConnector connector, Type contractType, Type entityType, string targetFolder);
    Task GenerateEnvelopeAsync(IConnector connector, Type contractType, Type envelopeType, string targetFolder, HashSet<Type> envelopes, HashSet<Type> entities);
}
