using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchDemo;


[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ProductRepository _productRepository;

    public ProductController(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpPost]
    public async Task<IActionResult> IndexProduct([FromBody] Product product)
    {
        await _productRepository.IndexProductAsync(product);
        return Ok("Product indexed successfully");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(string id)
    {
        var product = await _productRepository.GetProductAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("search/{keyword}")]
    public async Task<IActionResult> SearchProducts(string keyword)
    {
        var results = await _productRepository.SearchProductsAsync(keyword);
        return Ok(results);
    }
}
