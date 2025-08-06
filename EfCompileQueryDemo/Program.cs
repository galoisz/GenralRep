using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

var builder = WebApplication.CreateBuilder(args);

// Configure EF Core with SQLite (or InMemory for testing)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=products.db")); // Use SQLite

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/products", async (AppDbContext db) =>
{
    var products = new List<Product>();

    await foreach (var product in db.GetProductsCompiled())
    {
        products.Add(product);
    }

    return Results.Ok(products);
});

app.MapPost("/products", async (AppDbContext db, [FromBody] Product product) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.Run();

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    private static readonly Func<AppDbContext, IAsyncEnumerable<Product>> _compiledQuery =
        EF.CompileAsyncQuery((AppDbContext context) => context.Products.AsNoTracking());

    public IAsyncEnumerable<Product> GetProductsCompiled() => _compiledQuery(this);
}
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

