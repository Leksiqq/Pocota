using ContosoPizza;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Net.Leksi.Pocota.Server;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPizza(typeof(PizzaService), serv =>
{
    //serv.AddScoped<PizzaDbContext, PizzaContextImpl>();
    serv.AddSqlServer<PizzaContextImpl>("Server=.\\sqlexpress;Database=ContosoPizza;Trusted_Connection=True;Encrypt=no;");
    //serv.AddSqlite<PizzaDbContext>("Data Source=ContosoPizza.db");
    serv.AddKeyedScoped<IAccessCalculator>(typeof(Pizza), (s, k) => new PizzaAccess(s));
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Add the CreateDbIfNotExists method call
app.CreateDbIfNotExists();

app.MapGet("/", () => @"Contoso Pizza management API. Navigate to /swagger to open the Swagger test UI.");

app.Run();

