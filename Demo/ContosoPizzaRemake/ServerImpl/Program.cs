using ContosoPizza;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPizza(typeof(PizzaService), serv =>
{
    serv.AddSqlite<PizzaContext>("Data Source=ContosoPizza.db");
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

