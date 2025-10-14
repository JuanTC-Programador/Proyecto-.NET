using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;
using ProductApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRepository<Product>, Repository<Product>>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok("API funcionando (.NET 9 Minimal API)"));

app.MapGet("/products", async (IRepository<Product> repo) =>
{
    var all = await repo.GetAll();
    return Results.Ok(all);
});

app.MapGet("/products/{id:int}", async (int id, IRepository<Product> repo) =>
{
    var p = await repo.GetById(id);
    return p is not null ? Results.Ok(p) : Results.NotFound();
});

app.MapPost("/products", async (IRepository<Product> repo, Product product) =>
{
    if (string.IsNullOrWhiteSpace(product.Name))
        return Results.BadRequest(new { error = "Name is required" });

    if (product.Price <= 0)
        return Results.BadRequest(new { error = "Price must be greater than 0" });

    var created = await repo.Add(product);
    return Results.Created($"/products/{created.Id}", created);
});

app.MapPut("/products/{id:int}", async (int id, Product updated, IRepository<Product> repo) =>
{
    var product = await repo.GetById(id);
    if (product is null) return Results.NotFound();

    product.Name = updated.Name;
    product.Price = updated.Price;
    await repo.Update(product);

    return Results.NoContent();
});

app.MapDelete("/products/{id:int}", async (int id, IRepository<Product> repo) =>
{
    var product = await repo.GetById(id);
    if (product is null) return Results.NotFound();

    await repo.Delete(id);
    return Results.NoContent();
});

await app.RunAsync();



