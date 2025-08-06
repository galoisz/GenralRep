using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;

namespace ElasticSearchDemo;

public class ElasticSearchSettings
{
    public string Uri { get; set; } = string.Empty;
}


public class ElasticsearchService
{
    private readonly ElasticClient _client;

    public ElasticsearchService(IOptions<ElasticSearchSettings> settings)
    {
        //var settingsConfig = new ConnectionSettings(new Uri(settings.Value.Uri))
        //    .DefaultIndex("products"); // Default index

        var settingsConfig = new ConnectionSettings(new Uri(settings.Value.Uri)); // Default index


        _client = new ElasticClient(settingsConfig);
    }

    public ElasticClient GetClient() => _client;
}



