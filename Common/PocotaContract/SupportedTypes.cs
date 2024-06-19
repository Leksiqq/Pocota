using System.Collections.ObjectModel;

namespace Net.Leksi.Pocota.Contract;
public static class SupportedTypes
{
    public static ReadOnlyCollection<Type> Types { get; private set; } = new([
        typeof(string),
        typeof(decimal),
        typeof(ICollection<>),
        typeof(DateTime),
        typeof(DateOnly),
        typeof(TimeSpan),
        typeof(TimeOnly),
    ]);
}
