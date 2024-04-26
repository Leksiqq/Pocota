using ContosoPizza.Models;

namespace ContosoPizza;

public class PizzaAccess : PizzaAccessBase
{
    public PizzaAccess(IServiceProvider services) : base(services)
    {
    }

    protected override void DoCalculate(Pizza entity, PizzaPocotaEntity pocotaEntity)
    {
        pocotaEntity.Name.Access = Net.Leksi.Pocota.Contract.AccessKind.Forbidden;
    }
}
