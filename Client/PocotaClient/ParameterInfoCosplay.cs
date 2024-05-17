using System.Reflection;

namespace Net.Leksi.Pocota.Client;

public class ParameterInfoCosplay: ParameterInfo
{
    private readonly string _name;
    private readonly Type _type;
    public override string? Name => _name;
    public override Type ParameterType => _type;
    public ParameterInfoCosplay(string name, Type type)
    {
        _name = name;
        _type = type;
    }
}
