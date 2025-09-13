using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private static readonly List<ProductEntry> _products = new();
        private static int _nextId = 1;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProductEntry>> GetAllProducts()
        {
            _logger.LogDebug("Debug: Starting GetAllProducts method");
            _logger.LogInformation("Getting all products. Total count: {Count}", _products.Count);
            return Ok(_products);
        }

        [HttpGet("{id}")]
        public ActionResult<ProductEntry> GetProduct(int id)
        {
            _logger.LogInformation("Getting product with ID: {ProductId}", id);
            
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return NotFound($"Product with ID {id} not found");
            }

            _logger.LogInformation("Successfully retrieved product: {ProductName} (ID: {ProductId})", product.Name, product.Id);
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<ProductEntry> CreateProduct(ProductEntry product)
        {
            _logger.LogInformation("Creating new product: {ProductName}", product.Name);
            
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                _logger.LogWarning("Attempted to create product with empty name");
                return BadRequest("Product name is required");
            }

            product.Id = _nextId++;
            _products.Add(product);
            
            _logger.LogInformation("Successfully created product: {ProductName} with ID: {ProductId}", product.Name, product.Id);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductEntry updatedProduct)
        {
            _logger.LogInformation("Updating product with ID: {ProductId}", id);
            
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Attempted to update non-existent product with ID: {ProductId}", id);
                return NotFound($"Product with ID {id} not found");
            }

            if (string.IsNullOrWhiteSpace(updatedProduct.Name))
            {
                _logger.LogWarning("Attempted to update product {ProductId} with empty name", id);
                return BadRequest("Product name is required");
            }

            var oldName = existingProduct.Name;
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Description = updatedProduct.Description;
            
            _logger.LogInformation("Successfully updated product ID: {ProductId}. Name changed from '{OldName}' to '{NewName}'", 
                id, oldName, existingProduct.Name);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _logger.LogInformation("Deleting product with ID: {ProductId}", id);
            
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                _logger.LogWarning("Attempted to delete non-existent product with ID: {ProductId}", id);
                return NotFound($"Product with ID {id} not found");
            }

            _products.Remove(product);
            _logger.LogInformation("Successfully deleted product: {ProductName} (ID: {ProductId})", product.Name, product.Id);
            
            return NoContent();
        }

        [HttpGet("test-logs")]
        public IActionResult TestLogs()
        {
            _logger.LogDebug("This is a DEBUG log - should go to 'debug' queue");
            _logger.LogInformation("This is an INFO log - should go to 'info' queue");
            _logger.LogWarning("This is a WARNING log - should go to 'info' queue");
            _logger.LogError("This is an ERROR log - should go to 'errors' queue");
            _logger.LogCritical("This is a CRITICAL log - should go to 'errors' queue");

            try
            {
                throw new InvalidOperationException("Test exception for logging");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during test - should go to 'errors' queue");
            }

            return Ok(new { message = "Logs sent to RabbitMQ with different levels", timestamp = DateTime.UtcNow });
        }
    }
}

