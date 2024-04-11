using ContosoPizza.Models;

namespace ContosoPizza;
public class PizzaService : PizzaServiceBase
{
    private readonly IServiceProvider _services;
    private readonly PizzaContext _dbContext;
    public PizzaService(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaContext>();
    }
    public override IAsyncEnumerable<Pizza> FindPizzasAsync(PizzaFilter filter)
    {
        throw new NotImplementedException();
    }

    public override IAsyncEnumerable<Pizza> GetAllPizzasAsync()
    {
        throw new NotImplementedException();
    }

    public override IAsyncEnumerable<Sauce> GetAllSaucesAsync()
    {
        return _dbContext.SetOfSauce.AsAsyncEnumerable();
    }

    public override IAsyncEnumerable<Topping> GetAllToppingsAsync()
    {
        throw new NotImplementedException();
    }

    public override ValueTask<Pizza> GetPizzaAsync(Pizza pizza)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<Sauce> GetSauceAsync(Sauce sauce)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<Topping> GetToppingAsync(Topping topping)
    {
        throw new NotImplementedException();
    }
}