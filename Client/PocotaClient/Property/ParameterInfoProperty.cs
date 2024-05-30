using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class ParameterInfoProperty: Property
{
    private readonly bool _isNullable;
    public override bool IsNullable => _isNullable;
    internal ParameterInfoProperty(ParameterInfo info) : base(info.Name!, info.ParameterType) 
    {
        NullabilityInfoContext nic = new();
        _isNullable = nic.Create(info).ReadState is NullabilityState.Nullable;
    }
}
