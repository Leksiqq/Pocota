using Net.Leksi.Pocota.Contract;

namespace Net.Leksi.Pocota.Server;

public interface IAccessCalculator
{
    AccessKind Calculate(object entity);
}
