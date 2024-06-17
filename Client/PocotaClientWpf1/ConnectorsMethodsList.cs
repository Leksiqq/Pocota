using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Reflection;

namespace Net.Leksi.Pocota.Client;

public class ConnectorsMethodsList: IEnumerable<ConnectorMethod>
{
    private List<ConnectorMethod>? _methods = null;
    private readonly List<Type> _types = [];
    internal IServiceProvider Services { get; set; } = null!;
    public ConnectorMethod? this[MethodInfo methodInfo] => Methods.Where(cm => cm.Method == methodInfo).FirstOrDefault();
    public IEnumerator<ConnectorMethod> GetEnumerator()
    {
        return Methods.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Methods.GetEnumerator();
    }
    private List<ConnectorMethod> Methods
    {
        get
        {
            if(_methods is null)
            {
                _methods = new List<ConnectorMethod>();
                foreach (Type type in _types)
                {
                    if(Services.GetServices(type) is IEnumerable<object?> items)
                    {
                        foreach(object? obj in items)
                        {
                            if(obj is Connector conn)
                            {
                                foreach (MethodInfo method in conn.GetType().GetMethods().Where(m => m.DeclaringType == type && m.Name != nameof(Connector.GetPocotaConfigAsync)))
                                {
                                    _methods.Add(new ConnectorMethod(conn, method));
                                }
                            }
                        }
                    }
                }
            }
            return _methods;
        }
    }
    internal void AddConnectorType(Type type)
    {
        _types.Add(type);
    }
}