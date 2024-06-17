using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class ParameterInfoProperty: Property
{
    private readonly bool _isNullable;
    private readonly ParameterInfo _info;
    public override bool IsNullable => _isNullable;
    public override object? Declarator => _info.Member;
    internal ParameterInfoProperty(ParameterInfo info) : base(info.Name!, info.ParameterType) 
    {
        _info = info;
        NullabilityInfoContext nic = new();
        _isNullable = nic.Create(info).ReadState is NullabilityState.Nullable;
    }
}
