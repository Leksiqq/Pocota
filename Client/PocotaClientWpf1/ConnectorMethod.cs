using System.Reflection;

namespace Net.Leksi.Pocota.Client;

public class ConnectorMethod
{
    public string Name => Method.Name;
    public Type DeclaringType => Connector.GetType();
    public Module Module => Method.Module;
    public string ConnectorName => Connector.Name;
    internal Connector Connector { get; private init; }
    internal MethodInfo Method { get; private init; }
    internal ConnectorMethod(Connector connector, MethodInfo methodInfo)
    {
        Connector = connector;
        Method = methodInfo;
    }
}