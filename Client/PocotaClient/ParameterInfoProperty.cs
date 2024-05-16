using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class ParameterInfoProperty: Property
{
    internal ParameterInfoProperty(ParameterInfo info) : base(info.Name!, info.ParameterType) { }
}
