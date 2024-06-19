namespace Net.Leksi.Pocota.Contract;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
public class EnvelopeAttribute: Attribute
{
    private readonly Type _envelopeType;
    public Type EnvelopeType => _envelopeType;
    public EnvelopeAttribute(Type envelopeType)
    {
        _envelopeType = envelopeType;
        if (!_envelopeType.IsClass)
        {
            throw new ArgumentException($"{_envelopeType} is not a class.");
        }
    }
}
