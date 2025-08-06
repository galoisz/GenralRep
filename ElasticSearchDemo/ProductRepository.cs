using Nest;

namespace ElasticSearchDemo;

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
}

public class ProductRepository
{
    private readonly ElasticClient _client;

    public ProductRepository(ElasticsearchService esService)
    {
        _client = esService.GetClient();
    }

    public async Task IndexProductAsync(Product product)
    {
        await _client.IndexDocumentAsync(product);
    }

    public async Task<Product?> GetProductAsync(string id)
    {
        var response = await _client.GetAsync<Product>(id);
        return response.Source;
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword)
    {
        var response = await _client.SearchAsync<Product>(s => s
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Name)
                    .Query(keyword)
                ) 
            )
        );

        return response.Documents;
    }
}
