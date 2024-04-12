namespace Net.Leksi.Pocota.Contract;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class PocotaContractAttribute(string? contractName = null): Attribute
{
    public string? ContractName => contractName;
}
