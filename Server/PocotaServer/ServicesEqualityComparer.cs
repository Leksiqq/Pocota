using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Net.Leksi.Pocota.Server;

public class AccessCalculatorServicesEqualityComparer : IEqualityComparer<ServiceDescriptor>
{
    public static AccessCalculatorServicesEqualityComparer Instance { get; private set; } = new();

    private AccessCalculatorServicesEqualityComparer() { }
    public bool Equals(ServiceDescriptor? x, ServiceDescriptor? y)
    {
        return x?.ServiceType == typeof(IAccessCalculator) && (x?.IsKeyedService ?? false)
            && y?.ServiceType == typeof(IAccessCalculator) && (y?.IsKeyedService ?? false)
            && object.ReferenceEquals(x?.ServiceKey, y?.ServiceKey);
    }

    public int GetHashCode([DisallowNull] ServiceDescriptor obj)
    {
        return HashCode.Combine(obj.IsKeyedService, obj.ServiceKey, obj.ServiceType);
    }
}
