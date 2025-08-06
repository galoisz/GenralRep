using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Diagnostics;

// Define Entity
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Define DbContext with an Interceptor
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new QueryPerformanceInterceptor());
    }
}

// Define a Query Performance Interceptor
public class QueryPerformanceInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        Debug.WriteLine($"Executing Query: {command.CommandText}");
        return base.ReaderExecuting(command, eventData, result);
    }
}

// Repository
public class ProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }
}

// Program Entry Point
class Program
{
    static async Task Main()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=products.db")
            .Options;

        using var context = new AppDbContext(options);

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        var repo = new ProductRepository(context);
        var products = await repo.GetProductsAsync();

        Console.WriteLine("Products Loaded: " + products.Count);
    }
}
