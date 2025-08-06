using Microsoft.AspNetCore.Mvc;
using MVCWebApp.Models;

namespace MvcApp.Controllers;

public class ProductController : Controller
{
    private static List<Product> _products = new List<Product>
    {
        new Product { Id = 1, Name = "Laptop", Price = 1000 },
        new Product { Id = 2, Name = "Phone", Price = 500 }
    };

    public IActionResult Index()
    {
        return View(_products);
    }

    [HttpGet]
    public IActionResult Search(string query)
    {
        var filteredProducts = string.IsNullOrEmpty(query)
            ? _products
            : _products.Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        return View("Index", filteredProducts);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        product.Id = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1;
        _products.Add(product);
        return RedirectToAction("Index");
    }
}
