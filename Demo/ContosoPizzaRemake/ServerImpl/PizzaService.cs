﻿using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza;
public class PizzaService : PizzaServiceBase
{
    private readonly IServiceProvider _services;
    private readonly PizzaDbContext _dbContext;
    public PizzaService(IServiceProvider services)
    {
        _services = services;
        _dbContext = _services.GetRequiredService<PizzaDbContext>();
    }
    public override IAsyncEnumerable<Pizza> FindPizzasAsync(PizzaFilter filter)
    {
        throw new NotImplementedException();
    }

    public override IAsyncEnumerable<Pizza> GetAllPizzasAsync()
    {
        return _dbContext.SetOfPizza
            //.Include(p => p.Sauce)
            .Include(p => p.Toppings)!
            .AsAsyncEnumerable();
    }

    public override IAsyncEnumerable<Sauce> GetAllSaucesAsync()
    {
        return _dbContext.SetOfSauce.AsAsyncEnumerable();
    }

    public override IAsyncEnumerable<Topping> GetAllToppingsAsync()
    {
        throw new NotImplementedException();
    }

    public override ValueTask<Pizza?> GetPizzaAsync(Pizza pizza)
    {
        return _dbContext.SetOfPizza.FindAsync(pizza.Id);
    }

    public override ValueTask<Sauce?> GetSauceAsync(Sauce sauce)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<Topping?> GetToppingAsync(Topping topping)
    {
        throw new NotImplementedException();
    }
}